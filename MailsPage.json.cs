using Starcounter;

namespace MultiplePagesDemo
{
    partial class MailsPage : Json
    {
    }

    [MailsPage_json.Mails]
    partial class mailselement : Json, IBound<Mail>
    {
    }
}
