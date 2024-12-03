namespace BO
{
    /// <summary>
    /// Represents the details of a call assignment in the list.
    /// </summary>
    /// <param name="VolunteerId">ID of the volunteer assigned to the call</param>
    /// <param name="VolunteerName">Name of the volunteer assigned to the call</param>
    /// <param name="StartTreatmentTime">Time when the volunteer started handling the call</param>
    /// <param name="EndTreatmentTime">Actual end time of the treatment</param>
    /// <param name="EndType">Type of the treatment end (ENUM)</param>
    public class CallAssignInList
    {
        public int? VolunteerId { get; set; }
        public string? VolunteerName { get; set; }
        public DateTime StartTreatmentTime { get; set; }
        public DateTime? EndTreatmentTime { get; set; }
        public MyFinishType? EndType { get; set; }
    }


}
