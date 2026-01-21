namespace ViewCounter.Domain.DTO
{
    public class PopularEntityDto
    {
        public string EntityId { get; set; }
        public int Views { get; set; }
        public DateTime LastViewedAt { get; set; }
    }

}
