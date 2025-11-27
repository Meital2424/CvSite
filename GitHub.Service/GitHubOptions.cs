using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// אנו טוענים את שם המשתמש והטוקן הסודי מתוך secrets.json (סביבת פיתוח) או Configuration אחר.
//IOptions<GitHubOptions> מוזרק לשירותים כדי לגשת לנתונים אלו באופן מאובטח.
namespace GitHub.Service;

public class GitHubOptions
{
    // משמש כמפתח הראשי ב-secrets.json
    public const string GitHub = "GitHub";

    public string Username { get; set; } = string.Empty;
    public string PersonalAccessToken { get; set; } = string.Empty;
}
