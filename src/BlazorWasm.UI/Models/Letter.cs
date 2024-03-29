namespace POC_AnotherType.Models
{
    public class Letter
    {
        public Letter(char character, LetterStatus status)
        {
            Character = character;
            Status = status;
        }

        public char Character { get; set; }
        public LetterStatus Status { get; set; } = LetterStatus.None;
    }
}
