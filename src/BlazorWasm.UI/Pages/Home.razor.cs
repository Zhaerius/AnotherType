using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using POC_AnotherType.Models;
using POC_AnotherType.Services;

namespace POC_AnotherType.Pages
{
    public class HomeBase : ComponentBase
    {
        protected ElementReference InputDiv;
        protected bool Display = true;
        protected bool IsRestarted;
        protected TypingSession TypingSession { get; private set; } = new();
        [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
        [Inject] private FakerService FakerService { get; set; } = null!;

        protected override void OnInitialized()
        {
            SetLetters();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await UpdateCaretPosition(true);
            }
        }

        protected async Task Restart()
        {
            Console.WriteLine("Restart");
            
            IsRestarted = true;
            Display = false;
            TypingSession = new TypingSession();
            SetLetters();
            await UpdateCaretPosition();
            Display = true;
        }
        
        protected async Task OnFocusOut()
        {
            if (!IsRestarted)
            {
                await Task.Delay(250);
                TypingSession.IsFocus = false;
            }
            else
            {
                await InputDiv.FocusAsync();
                IsRestarted = false;
            }
        }

        protected string GetWpm()
        {
            var wpm = TypingSession.CalcWpm();
            return wpm.ToString("0") + " WPM";
        }
        
        protected string GetAcc()
        {
            var acc = TypingSession.CalcAcc();
            return acc.ToString("0") + "%";
        }

        protected async Task HandleLetter(KeyboardEventArgs e)
        {
            // No handler if typing is finish
            if (TypingSession.IsFinish)
                return;

            // Guard for remove key like SHIFT, FUNCTIONS, ARROW, Etc...
            if (e.Key.Length != 1 && e.Key != "Backspace")
                return;

            // Backspace press
            if (e.Key == "Backspace")
            {
                // Lock word, no backspace after space (word validation)
                if (TypingSession.Index > 0 && TypingSession.Letters[TypingSession.Index - 1].Character != ' ')
                {
                    if (!TypingSession.IsCursorValid)
                    {
                        TypingSession.IsCursorValid = true;
                        return;
                    }

                    TypingSession.Index--;
                    await UpdateCaretPosition();
                    TypingSession.IsCursorValid = true;
                    TypingSession.UserText = TypingSession.UserText.Remove(TypingSession.UserText.Length - 1, 1);
                    TypingSession.Letters[TypingSession.Index].Status = LetterStatus.None;
                }
            }
            else if (e.Key != " " && TypingSession.Letters[TypingSession.Index].Character == ' ')
            {
                // Guard for space word
                TypingSession.Errors++;
                TypingSession.IsCursorValid = false;
            }
            else
            {
                // Normal use case
                var isCorrect = e.Key[0] == TypingSession.Letters[TypingSession.Index].Character;

                if (!isCorrect)
                    TypingSession.Errors++;

                if (TypingSession.Index == 0)
                    TypingSession.Start = DateTime.Now;

                TypingSession.IsCursorValid = true;
                TypingSession.UserText += e.Key;
                TypingSession.Letters[TypingSession.Index].Status = isCorrect ? LetterStatus.Right : LetterStatus.Wrong;
                TypingSession.Index++;

                if (TypingSession.Index != TypingSession.Letters.Count)
                    await UpdateCaretPosition();
                else
                {
                    TypingSession.IsFinish = true;
                    TypingSession.End = DateTime.Now;
                }
            }               
        }

        private void SetLetters()
        {
            TypingSession.BaseText = FakerService.GetText();
            foreach (var letterStr in TypingSession.BaseText)
            {
                var letterObj = new Letter(letterStr, LetterStatus.None);
                TypingSession.Letters.Add(letterObj);
            }
        }

        private async Task UpdateCaretPosition(bool firstRender = false)
        {
            if (!firstRender)
                TypingSession.IsTyping = false;
            
            var coordinates = await JsRuntime.InvokeAsync<CaretCoordinates>("GetCoordinatesLetter", InputDiv, TypingSession.Index);
            TypingSession.StyleCaret = $"left: {coordinates.Left}px;top: {coordinates.Top + 8}px;";

            await InputDiv.FocusAsync(true);
        }
    }
}
