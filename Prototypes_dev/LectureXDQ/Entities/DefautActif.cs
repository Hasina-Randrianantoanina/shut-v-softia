namespace LectureXDQ.Entities
{
    public class DefautActif
    {
        public int Id { get; set; }
        public DateTime Appel {  get; set; }
        public string Type { get; set; } // Capteur only
        public bool Active { get; set; } = true; // Not in db, used to insert/delete defauts

    }
}
