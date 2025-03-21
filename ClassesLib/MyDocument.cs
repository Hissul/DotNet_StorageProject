namespace ClassesLib
{
    public class MyDocument // нужен
    {
        public string DocumentType { get; set; } = "";
        public string DocumentNumber { get; set; } = "";
        public string ProductName { get; set; } = "";
        public int Amount { get; set; }
        public DateTime Date { get; set; }
        public string EmployeeSurname { get; set; } = "";
        public string Party { get; set; } = "";

        public MyDocument(){}

        public MyDocument(string documentType, string documentNumber, string productName, int amount, DateTime date, string employeeSurname, string party)
        {
            DocumentType = documentType;
            DocumentNumber = documentNumber;
            ProductName = productName;
            Amount = amount;
            Date = date;
            EmployeeSurname = employeeSurname;
            Party = party;
        }
    }
}
