namespace OrderManagementUI.Models
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public T Result { get; set; }
        public string DisplayMessage { get; set; }
    }

}
