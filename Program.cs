using Starcounter;
using System.Linq;

namespace MultiplePagesDemo
{
    [Database]
    public class Mail
    {
        public string Title;
        public string Content;
        public string Url => "/MultiplePagesDemo/mails/" + this.GetObjectID();
    }

    public class Program
    {
        static void Main()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/MultiplePagesDemo/mails", () =>
            {
                if (Session.Current != null)
                {
                    return Session.Current.Data;
                }
                Session.Current = new Session(SessionOptions.PatchVersioning);

                var mailPage = new MailsPage()
                {
                    Session = Session.Current,
                    Mails = Db.SQL<Mail>("SELECT m FROM MultiplePagesDemo.Mail m")
                };

                Focused foc = new Focused();
                foc.Data = mailPage.Mails.FirstOrDefault();
                mailPage.Focused = foc;

                return mailPage;
            });

            Handle.GET("/multiplepagesdemo/mails/{?}", (string id) =>
            {
                Mail mail = Db.SQL<Mail>("SELECT m FROM multiplepagesdemo.mail m WHERE objectid=?", id).First;
                MailsPage mp = Self.GET<MailsPage>("/multiplepagesdemo/mails");
                mp.Focused.Data = mail;
                return mp;
            });

            Db.Transact(() =>
            {
                if (Db.SQL<long>("SELECT COUNT(m) FROM multiplepagesdemo.mail m").First == 0)
                {
                    new Mail()
                    {
                        Title = "Hello Mail",
                        Content = "This is my first email!"
                    };

                    new Mail()
                    {
                        Title = "Greetings",
                        Content = "How are you? Regards jack"
                    };
                }
            });
        }
    }
}