using System;
using System.Collections.Generic;
using Unity.UIWidgets.async;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.rendering;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.widgets;

namespace PomoTimerApp
{
    public class TimerPage : StatefulWidget
    {
        public Task TaskData { get; }

        public TimerPage(Task taskData)
        {
            this.TaskData = taskData;
        }

        public override State createState()
        {
            return new TimerPageState();
        }
    }

    class TimerPageState : State<TimerPage>
    {
        public readonly static TimeSpan DELAY = TimeSpan.FromMilliseconds(100);

        private string mTimeText   = "25:00";
        private string mButtonText = "START";

        private Timer          mTimer     = null;
        private StopwatchTimer mStopwatch = null;

        public override void initState()
        {
            base.initState();

            mStopwatch = new StopwatchTimer(30);

            mTimer = Window.instance.periodic(DELAY, () =>
            {
                var minutes = mStopwatch.TotalMinutes;

                if (minutes == 25)
                {
                    UnityEngine.Debug.Log("到达 25 分钟");

                    this.setState(() => { mTimeText = "00:00"; });

                    if (Navigator.canPop(context))
                    {
                        widget.TaskData.PomoCount++;
                        Navigator.pop(context, widget.TaskData);
                    }

                    return;
                }

                UnityEngine.Debug.LogFormat("DELAYED:{0}", mStopwatch.TotalSeconds);


                var minutsText = (25 - mStopwatch.Minutes - 1).ToString().PadLeft(2, '0');
                var secondsText = (60 - mStopwatch.Seconds - 1).ToString().PadLeft(2, '0');

                mTimeText = $"{minutsText}:{secondsText}";

                if (mStopwatch.IsRunning)
                {
                    this.setState(() => { mButtonText = "RUNNING"; });
                }
                else if ((int) mStopwatch.TotalSeconds == 0)
                {
                    this.setState(() => { mTimeText = "25:00"; });
                }
                else
                {
                    this.setState(() => { mButtonText = "PAUSED"; });
                }
            });
        }

        public override void dispose()
        {
            base.dispose();

            mStopwatch.Stop();
            mTimer.cancel();
        }

        public override Widget build(BuildContext context)
        {
            return new Scaffold(
                backgroundColor: Colors.white,
                body: new Material(
                    child: new Stack(
                        children: new List<Widget>()
                        {
                            new Padding(
                                padding: EdgeInsets.only(top: 32, left: 4, right: 4),
                                child: new Row(
                                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                                    children: new List<Widget>()
                                    {
                                        new IconButton(
                                            icon: new Icon(Icons.arrow_back,
                                                color: Colors.grey,
                                                size: 40
                                            ),
                                            onPressed: () =>
                                            {
                                                if (mStopwatch.Minutes > 0)
                                                {

                                                    widget.TaskData.PomoCount++;
                                                    Navigator.pop(context, widget.TaskData);
                                                }
                                                else
                                                {
                                                    Navigator.pop(context);
                                                }
                                            }
                                        ),
                                        new IconButton(
                                            icon: new Icon(Icons.done_all,
                                                color: Colors.red,
                                                size: 40
                                            ),
                                            onPressed: () =>
                                            {
                                                widget.TaskData.Done = true;
                                                Navigator.pop(context, widget.TaskData);
                                            }
                                        )
                                    }
                                )
                            ),
                            new Align(
                                alignment: Alignment.topCenter,
                                child: new Container(
                                    margin: EdgeInsets.only(top: 100),
                                    child: new Column(
                                        mainAxisSize: MainAxisSize.max,
                                        children: new List<Widget>
                                        {
                                            new Text(widget.TaskData.Title, style: new TextStyle(
                                                color: Colors.grey,
                                                fontSize: 30
                                            ))
                                        }
                                    )
                                )
                            ),
                            new Align(
                                alignment: Alignment.center,
                                child: new Text(mTimeText, style: new TextStyle(
                                        color: Colors.black,
                                        fontSize: 54,
                                        fontWeight: FontWeight.bold
                                    )
                                )),
                            new Align(
                                alignment: Alignment.bottomCenter,
                                child: new Container(
                                    margin: EdgeInsets.only(bottom: 32),
                                    child: new GestureDetector(
                                        child: new RoundedButton(mButtonText),
                                        onTap: () =>
                                        {
                                            if (mStopwatch.IsRunning)
                                            {
                                                mStopwatch.Stop();
                                            }
                                            else
                                            {
                                                mStopwatch.Start();
                                            }
                                        }
                                    )
                                )
                            )
                        }
                    )
                )
            );
        }
    }
}