namespace StudyFlow.Models;
public class StudySessionParticipants
{

    //Organizes participants of a study session, linking to session and keeping track of users. 
    public int Id {get; set;}
    public int StudySessionId {get; set;}
    public string UserId {get; set;}

    public bool IsHost {get; set;} = false; 

}