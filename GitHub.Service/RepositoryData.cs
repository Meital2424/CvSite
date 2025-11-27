namespace GitHub.Service;

// תפקיד: מודל נקי ומופשט (POCO) של הנתונים שאנו רוצים להחזיר ללקוח.
// חשיבות: אנו לא מחזירים את האובייקטים הגולמיים של Octokit (שיש בהם המון מידע מיותר). במקום זאת, אנו ממפים רק את הנתונים הדרושים (שם, שפות, תאריך עדכון, כוכבים) למודל זה.
public class RepositoryData
{
    public string Name { get; set; } = string.Empty;
    public IReadOnlyList<string> Languages { get; set; } = new List<string>();
    public DateTime LastCommitDate { get; set; }
    public int StargazersCount { get; set; }
    public int PullRequestsCount { get; set; }
    public string? HomepageUrl { get; set; }
}