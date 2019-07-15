using System.Diagnostics;
using System.Text;
using System.Threading;

namespace WF_iexplorer
{
    class Controller
    {
        StringBuilder Principal;
        private static Semaphore _pool;
        public Controller(Semaphore pool, ref StringBuilder strB)
        {
            _pool = pool;
            Principal = strB;
        }
        Youtube Youtube_o = null;
        enum States
        {
            BeforeStart,
            Started,
            Results,
            Video
        };
        static States GeneralState = States.BeforeStart;
        public void Run()
        {
            //Communication_And_Control();
            //var tk = new Task(Central_Head);
            //tk.Start();
            var th = new Thread(Central_Head);
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
        }
        private SHDocVw.InternetExplorer AssociateProcessWithIe(out Process p)
        {
            p = new Process();
            p.StartInfo.FileName = "iexplore.exe";
            p.StartInfo.Arguments = "-nomerge";
            p.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            p.Start();

            SHDocVw.InternetExplorer IE = null;
            while (IE == null)
            {
                SHDocVw.ShellWindows everyBrowser = new SHDocVw.ShellWindows();
                var max = everyBrowser.Count;
                for (int i = 0; i < max; i++)
                {
                    SHDocVw.InternetExplorer e = (SHDocVw.InternetExplorer)everyBrowser.Item(i);
                    Debug.WriteLine($"ProcessWindow:{(int)p?.MainWindowHandle},{e?.HWND}");
                    if (e != null && (e.HWND == (int)p.MainWindowHandle))
                    {
                        IE = e;
                        return IE;
                    }
                }
                Thread.Sleep(1000);
            }
            return null;
        }
        void Central_Head()
        {

            SHDocVw.InternetExplorer IE = null;
            Process p = null;
            while (true)
            {
                _pool.WaitOne();
                var Str_array = Principal.ToString().Split(separator: new char[] { ';' }, count: 4,options:System.StringSplitOptions.RemoveEmptyEntries);
                switch (GeneralState)
                {
                    case States.BeforeStart:
                        if (Str_array[0] == "Start")
                        {
                            IE = AssociateProcessWithIe(out p);
                            Youtube_o = new Youtube(ref IE);
                            GeneralState = States.Started;
                        }
                        break;
                    case States.Started:
                        if (Str_array[0] == "Search")
                        {
                            if (Str_array.Length == 2 || Str_array[1] != null)
                            {
                                Youtube_o.Search(Str_array[1]);
                                GeneralState = States.Results;
                            }
                        }

                        break;
                    case States.Results:
                        switch (Str_array[0])
                        {
                            case ("Next Result"):
                                {
                                    Youtube_o.SelectNext();
                                }
                                break;
                            case ("Previous Result"):
                                {
                                    Youtube_o.SelectBefore();
                                }
                                break;
                            case ("Search"):
                                {
                                    if (Str_array.Length == 2 || Str_array[1] != null)
                                    {
                                        Youtube_o.Search(Str_array[1]);
                                        GeneralState = States.Results;
                                    }
                                }
                                break;
                            case ("Click"):
                                {
                                    Youtube_o.Click();
                                }
                                break;
                            case ("Play/Pause"):
                                {
                                    Youtube_o.Play_Pause();
                                }
                                break;
                            case ("Next Video"):
                                {
                                    Youtube_o.NextVideo();
                                }
                                break;
                            case ("Full Screen"):
                                {
                                    Youtube_o.Complete_screen();
                                }
                                break;
                            case ("Close Advertisement"):
                                Youtube_o.PassAd?.click();
                                break;
                            case "Center":
                                Youtube_o.WindowDef();
                                break;
                            case "Previous Page":
                                Youtube_o.History_back();
                                break;
                            case "Next Page":
                                Youtube_o.History_forward();
                                break;
                            case "Close IE":
                                p.Kill();
                                GeneralState = States.BeforeStart;
                                break;
                            case "ScrollBy":
                                var coords = Str_array[1].Split(separator: new char[] { ';' }, count: 2);
                                int.TryParse(coords[0], out int x);
                                int.TryParse(coords[0], out int y);
                                Youtube_o.WindowScroll(x, y);
                                break;
                            default:
                                break;
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
