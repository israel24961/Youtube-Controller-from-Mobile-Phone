
using System;
using System.Drawing;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WF_iexplorer
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var tokenSource2 = new CancellationTokenSource();
            Form1 form1 = new Form1();
            Form2 form2 = new Form2();
            form2.FormClosed += (object obj, FormClosedEventArgs ev) =>
            {
                try
                {
                    form1?.p?.Kill();
                }
                catch (Exception)
                {

                }

                Environment.Exit(0);
            };
            form2.Show();
            form2.WindowState = FormWindowState.Minimized;
            var font = new Font(form1.Txt2Write.Font.FontFamily, 28);
            form1.Txt2Write.Font = font;
            form1.Txt2Write.Text = $"Write this in your browser: \n'{SocketController.getLocalIP()}/' \n\t(without the brackets nor spaces)";
            form1.Show();

            Semaphore _pool = new Semaphore(0, 1);

            StringBuilder stringBuilder = new StringBuilder(128);

            //Communication Layer
            var SC = new SocketController(IPAddress.Parse("127.0.0.128"), 34197, "|SocketControllernº1|:");
            SC.AddOnConnect((object o, MessageEventController.MessageConnect_EventArgs e) =>
                            {
                                if (form1 == null || form1.IsDisposed)
                                {
                                    return;
                                }
                                form1.Invoke(new Form1.ChangeLabel_safe((string txt) => { form1.Txt2Write.Text += txt; }), "\nWhen you start the browser this window will close");
                            });
            SC.AddOnReceive((object o, MessageEventController.MessageSend_EventArgs e) =>
                            {
                                stringBuilder.Clear();
                                stringBuilder.Insert(0, e.message);
                                try
                                {
                                    _pool.Release();
                                }
                                catch
                                {
                                    return;
                                }
                                if (e.message == "Start;" || e.message == "Start")
                                {
                                    if (form1 == null || form1.IsDisposed)
                                    {
                                        return;
                                    }
                                    form1.Invoke(new Form1.ChangeLabel_safe((string txt) => { form1.Txt2Write.Text = txt; form1.Close(); }), "Cya later");
                                }
                            });
            var Communication_thread = new Thread(() =>
                    {

                        SC.StartListening(10);

                    });
            Communication_thread.Start();


            //Starts IE in Youtube Page and parses messages to commands
            var Container = new Controller(_pool, ref stringBuilder);
            Container.Run();
            form1.Focus();
            //Makes iexplore.exe
            while (true)
            {
                Application.DoEvents();
                Thread.Sleep(500);
            }
        }
    }
}


