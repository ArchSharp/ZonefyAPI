namespace Domain.Entities
{
    public class Notification<T>
    {
        public string Type { get; set; }
        public T Data { get; set; }
    }
}