using Starcounter;

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
                if (Session.Current != null) return Session.Current.Data;

                Session.Current = new Session(SessionOptions.PatchVersioning);

                MailsPage mp = new MailsPage()
                {
                    Session = Session.Current,
                    Mails = Db.SQL<Mail>("SELECT m FROM MultiplePagesDemo.Mail m LIMIT ?", 10)
                };

                Focused foc = new Focused();
                foc.Data = mp.Mails[0].Data;
                mp.Focused = foc;

                return mp;
            });

            Handle.GET("/multiplepagesdemo/mails/{?}", (string id) =>
            {
                Mail mail = Db.SQL<Mail>("select m from multiplepagesdemo.mail m where objectid=?", id).First;
                MailsPage mp = Self.GET<MailsPage>("/multiplepagesdemo/mails");
                mp.Focused.Data = mail;
                return mp;
            });

            Db.Transact(() =>
            {
                Db.SlowSQL("DELETE FROM multiplepagesdemo.mail");

                new Mail()
                {
                    Title = "hello world",
                    Content = "this is my first email"
                };

                new Mail()
                {
                    Title = "hi alexey!",
                    Content = "how are you? regards jack"
                };
            });
        }
    }
}