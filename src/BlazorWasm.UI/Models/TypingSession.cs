namespace POC_AnotherType.Models
{
    public class TypingSession
    {
        public int Index { get; set; }
        public string BaseText { get; set; } = "";
        public string UserText { get; set; } = "";
        public string StyleCaret { get; set; } = "";
        public bool IsTyping { get; set; } = true;
        public bool IsFocus { get; set; } = true;
        public bool IsCursorValid { get; set; } = true;
        public bool IsFinish { get; set; }
        public int Errors { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<Letter> Letters { get; set; } = [];

        public float CalcAcc()
        {
            int totalErrors = GetErrors();

            float accuracy = ((float)BaseText.Length - totalErrors) / BaseText.Length;
            float accuracyPercent = accuracy * 100;

            return accuracyPercent;
        }

        public float CalcWpm()
        {
            const int word = 5;
            const int second = 60;

            var time = GetTypingTime();
            
            float numberWords = (float)BaseText.Length / word;
            float timeTyping = (float)time / second;

            return numberWords / timeTyping;
        }
        
        private int GetTypingTime()
        {
            if (Start >= End)
                throw new Exception("Impossible");
            
            var interval = End - Start;
            return interval.Seconds;
        }
        
        private int GetErrors()
        {
            int errorWithoutCorrection = 0;

            for (int i = 0; i < BaseText.Length; i++)
            {
                if (!string.Equals(BaseText[i], UserText[i]))
                {
                    errorWithoutCorrection++;
                }
            }

            return errorWithoutCorrection + Errors;
        }
    }
}
