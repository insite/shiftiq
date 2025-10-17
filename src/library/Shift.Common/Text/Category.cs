namespace Shift.Common
{
    public class Category
    {
        public string Name { get; set; }
        public int Number { get; set; }
        
        public Category(string text, int number)
        {
            Name = text;
            Number = number;
        }
    }
}
