# Youtube-Controller-from-Mobile-Phone (Since Window made the transition to CHROMIUM EDGE, it doesn't work)
Just a simple controller for Youtube, you can skip ads, search for videos, pause, play, next video,  and go through the recommended videos.
The flow of the program goes this way:
![alt text](https://github.com/israel24961/Youtube-Controller-from-Mobile-Phone/blob/master/Flow.png)

Link of the video: https://www.youtube.com/watch?v=er0glmJ1ueU <br>
1.-User goes to the web page that the app specifies, and introduce it in the 'connect' textBox, clicks it and displays a menu with buttons while openning the web explorer in the desktop. <br>
2.-User clicks on button <br>
3.-Button sends its command string through the WebSocket<br>
4.-Web server resend it to the local socket.<br>
5.-local socket sends it to the controller thread through the shared object "msg" and release the semaphore "_pool"

I've used both, a TCP socket(for inter communication process) and a websocket (for the webpage/server communication, instead of bare ajax), the Controller is written in .NET framework and the web server in .netcore 
