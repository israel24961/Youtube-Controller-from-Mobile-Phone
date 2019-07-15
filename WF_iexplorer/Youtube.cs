using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

namespace WF_iexplorer
{
    class Youtube
    {
        public enum TagReady
        {
            y_ready,
            n_ready
        }
        SHDocVw.InternetExplorer IE = null;
        private mshtml.IHTMLDocument3 DocHTML {
            get {
                return (dynamic)IE.Document;
            }
        }
        private mshtml.IHTMLWindow2 Window {
            get {
                var i1 = (mshtml.HTMLDocument)IE.Document;
                var i2 = (dynamic)(i1).parentWindow;
                return i2;
            }
        }
        private mshtml.HTMLInputButtonElement FullScreen_Button {
            get {
                var length_playerID = DocHTML.getElementById("movie_player").children.length;

                var RightControls = DocHTML.getElementById("movie_player")//player frame
                    .children[length_playerID - 1]//ytp-chrome-bottom
                    .children[1]//ytp-chrome-controls
                    .children[1];//ytp-right Controls
                var len = RightControls.children.length;
                return RightControls.children[len - 1];//full screen is the last element

            }
        }
        /// <summary>
        /// Button which can on the video element
        /// </summary>
        private mshtml.HTMLButtonElement Play_Pause_Button {
            get {
                var length_playerID = DocHTML.getElementById("movie_player").children.length;
                return DocHTML.getElementById("movie_player")//player frame
                    .children[length_playerID - 1]//ytp-chrome-bottom
                    .children[1]//ytp-chrome-controls
                    .children[0]//ytp-left Controls
                    .children[1];
            }
        }
        private mshtml.HTMLAnchorElement Next_Button {
            get {
                var length_playerID = DocHTML.getElementById("movie_player").children.length;
                return DocHTML.getElementById("movie_player")//player frame
                    .children[length_playerID - 1]//ytp-chrome-bottom
                    .children[1]//ytp-chrome-controls
                    .children[0]//ytp-left Controls
                    .children[2];
            }
        }
        /// <summary>
        /// SearchBar, where you write to, so you can search it.
        /// </summary>
        private mshtml.IHTMLInputElement SearchBar {
            get { return (mshtml.IHTMLInputElement)DocHTML.getElementById("masthead-search-term"); }
        }
        /// <summary>
        /// SearchButton (that you click after writing in the SearchBar)
        /// </summary>
        private mshtml.HTMLButtonElement SearchButton {
            get { return (mshtml.HTMLButtonElement)DocHTML.getElementById("search-btn"); }
        }
        /// <summary>
        /// Upper Youtube bar(where you see your account, search Bar, etc"
        /// </summary>
        private mshtml.HTMLDivElement YoutubeBar {
            get {
                return ((mshtml.HTMLDivElement)((mshtml.HTMLDocument)IE.Document).getElementById("yt-masthead-container"));
            }
        }
        /// <summary>
        /// Video recommendations of any Video you are watching
        /// </summary>
        private mshtml.HTMLUListElement RelatedList {
            get {
                return (dynamic)DocHTML.getElementById("watch-related");
            }
        }
        /// <summary>
        /// Result List, obtained as the list when you search for a word in Youtube
        /// Includes:
        ///     ·(Not filter)
        ///     ·The misspelling check('li')
        ///     ·The videos
        ///     ·Channels
        ///     ·"More" button at the end
        /// </summary>
        private mshtml.HTMLOListElement ResultList {
            get {
                return (dynamic)DocHTML.getElementById("results")?.children[0].children[1].children[0];
            }
        }
        /// <summary>
        /// Saves the position of the Selector(blue border)
        /// </summary>
        private int SelectPosition { get; set; }
        public void NextVideo()
        {
            Next_Button.click();
        }

        public void Play_Pause()
        {
            Play_Pause_Button.click();
        }
        public void Complete_screen()
        {
            ((mshtml.HTMLInputButtonElement)FullScreen_Button).click();
        }
        //returns the list (whether results from a search, or related videos from a video you are watching)
        private dynamic JustTheList(out int i)
        {

            object temp = null;
            i = 0;
            //RelateList 
            while (true)
            {
                if ((temp = RelatedList) != null)
                {
                    i = 0;
                    break;
                }
                //or Results
                else if ((temp = ResultList) != null)
                {
                    i = 1;
                    temp = ResultList;
                    break;
                }
            }
            //add the border to the element
            return ((dynamic)temp);
        }
        public void SelectNext()
        {
            dynamic temp = JustTheList(out int i);
            if (temp == null)
            {
                return;
            }
            //add the border to the element
            dynamic new_Element = ((dynamic)temp).children[SelectPosition];
            new_Element.style.border = "thick solid #0000FF";
            PutItemAtTop(new_Element);
            //clean before
            if (SelectPosition > 0)
            {
                mshtml.HTMLLIElement link_element = ((dynamic)temp).children[SelectPosition - 1];
                //remove border of current object
                link_element.style.border = "";
            }

            //whether there are more results
            if (SelectPosition < ((dynamic)temp).children.length)
            {
                SelectPosition++;
            }
            else
            {
                return;
            }
            Debug.WriteLine("finished SelectNext");
        }
        public void SelectBefore()
        {
            while (this.Ready != TagReady.y_ready || SearchBar == null)
            {
                Task.Delay(300).Wait();
            }
            dynamic temp = JustTheList(out int i);
            if (temp == null)
            {
                return;
            }
            if (SelectPosition > 0)
            {
                mshtml.HTMLLIElement link_element = ((dynamic)temp).children[--SelectPosition];
                //remove border of current object
                link_element.style.border = "";
                //add the border to the element
                if (SelectPosition > 0)
                {
                    var new_Element = ((dynamic)temp).children[SelectPosition - 1];
                    new_Element.style.border = "thick solid #0000FF";
                    PutItemAtTop(new_Element);

                }

            }
            Debug.WriteLine("finished SelectBefore");
        }
        public void Click()
        {
            while (this.Ready != TagReady.y_ready || SearchBar == null)
            {
                Task.Delay(300).Wait();
            }
            if (SelectPosition == 0) return;

            dynamic temp_list = JustTheList(out int i);
            if (temp_list == null)
            {
                return;
            }
            if (i == 0)
            {   //Related
                var reltemp = ((mshtml.HTMLUListElement)temp_list).children[SelectPosition - 1];
                //Related
                switch (reltemp.className)
                {
                    //video-playlist suggestions
                    case "video-list-item related-list-item  show-video-time related-list-item-compact-video":
                        reltemp.children[0].children[0].children[0].click();
                        break;
                    case "video-list-item related-list-item  show-video-time related-list-item-compact-radio":
                        reltemp.children[0].click();
                        break;
                    //more button
                    case "yt-uix-button yt-uix-button-size-default yt-uix-button-expander":
                        break;
                    default:
                        break;
                }
            }
            else
            {   //Result
                var reltemp = ((mshtml.HTMLOListElement)temp_list).children[SelectPosition - 1];//<li>
                switch (reltemp.children[0].className)//<div"yt_lu-tile">.className
                {
                    //Spell Correction
                    case "spell-correction spell-correction-dym":
                        reltemp.children[0]//<div>
                            .children[1].click();//<A> clickable
                        break;
                    //Video
                    case "yt-lockup yt-lockup-tile yt-lockup-playlist clearfix":
                        reltemp.children[0]//ty-lockup-tile
                            .children[0]//<div"yt-dismissable">
                            .children[0]//thumbnail
                            .children[0].click();//<a> click-able
                        break;
                    case "yt-lockup yt-lockup-tile yt-lockup-video clearfix":
                        reltemp.children[0]
                            .children[0]
                            .children[0]
                            .children[0].click();
                        break;
                    default:
                        break;
                };
            }
            SelectPosition = 0;
            Task.Delay(5000).Wait();
            Debug.WriteLine("finishedCLICK");
        }
        //Absolute scroll(upper left ==(0,0))
        public void WindowScroll(int x, int y)
        {
            Window.scrollBy(x, y);
        }
        public Point WindowXY()
        {
            return new Point(((dynamic)(Window)).pageXOffset, ((dynamic)((mshtml.IHTMLWindow2)Window)).pageYOffset);
        }
        public void WindowDef()
        {
            Window.scroll(0, 0);
        }
        /// <summary>
        /// In case you have started or attached an IE process
        /// 
        /// </summary>
        /// <param name="ie">InternetExplorer object(already initialized) where to start youtube Page</param>
        public Youtube(ref SHDocVw.InternetExplorer ie)
        {
            Launch(ref ie);
        }
        public Youtube()
        {
            var iE = new SHDocVw.InternetExplorer();
            Launch(ref iE);
        }
        private Task Launch(ref SHDocVw.InternetExplorer ie)
        {
            var loc = ie;
            loc.Visible = false;
            return Task.Run(() =>
            {
                loc.Navigate("www.youtube.com");
                while (loc.Busy || loc.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
                {
                    Task.Delay(100).Wait();
                }
                IE = loc;
                IE.Visible = true;

            });
        }
        public TagReady Ready {
            get {
                if (IE == null || IE.Busy || IE.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
                {
                    return TagReady.n_ready;
                }
                return TagReady.y_ready;
            }
        }
        /// <summary>
        /// Search the string <paramref name="text2Search"/>
        /// </summary>
        /// <param name="text2Search">String to put in the <seealso cref=""/></param>
        /// <returns></returns>
        public int Search(string text2Search)
        {
            int i = 1;
            if (this.Ready == TagReady.y_ready && SearchBar != null)
            {
                ((mshtml.IHTMLElement2)SearchBar).focus();
                SearchBar.value = text2Search;
                SearchButton.click();
                i = 0;
                SelectPosition = 0;
            }
            return i;
        }
        public void History_back()
        {
            IE?.GoBack();
        }
        public void History_forward()
        {
            IE?.GoForward();
        }
        public void PutItemAtTop(mshtml.IHTMLElement el)
        {
            var point = GetOffset(el);

            Window.scroll(point.X, point.Y - YoutubeBar.offsetHeight);
        }
        static public Point GetOffset(mshtml.IHTMLElement el)
        {
            //get element pos
            System.Drawing.Point pos = new System.Drawing.Point(el.offsetLeft, el.offsetTop);

            //get the parents pos
            mshtml.IHTMLElement tempEl = el.offsetParent;
            while (tempEl != null)
            {
                pos.X += tempEl.offsetLeft;
                pos.Y += tempEl.offsetTop;
                tempEl = tempEl.offsetParent;
            }

            return pos;
        }

        public mshtml.HTMLButtonElement PassAd {
            get {
                var childrens = (dynamic)DocHTML.getElementById("movie_player")?.children;
                if (childrens == null)
                {
                    return null;
                }
                int size = childrens.Length;

                for (int i = 0; i < size; i++)
                {
                    if (childrens[i].className == "video-ads ytp-ad-module")
                    {
                        return childrens[i]
                                ?.children[0]
                                ?.children[2]
                                ?.children[0]
                                ?.children[1]
                                ?.children[0]
                                ?.children[0];
                    }
                }
                return null;
            }

        }

        public void CloseAdRoutine()
        {
            Task.Run(() =>
            {
                var ADButton = PassAd;
                if (ADButton != null)
                {
                    ADButton.click();
                }
                Task.Delay(2000).Wait();
                CloseAdRoutine();
            });

        }
    }
}
