using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace jh_project
{
    public class JhCommandRunner
    {

        #region Constructors

        public JhCommandRunner()
        {
            ExecutableCommands = new List<IJhCommand>();
            RunningTasks = new List<Task>();
            StopSignalCounter = 0;
        }

        #endregion

        #region Properties

        private List<IJhCommand> ExecutableCommands {get; set;}
        private List<Task> RunningTasks { get; set; }
        private int StopSignalCounter { get; set; }

        #endregion

        #region Public Methods

        public void StackCommands()
        {
            SampleStreamReportCommand sampleStreamReportCommand = new SampleStreamReportCommand();
            sampleStreamReportCommand.Initialize();
            ExecutableCommands.Add(sampleStreamReportCommand);
        }

        public void Run()
        {
            RunEachCommandInNewThread();

            while(StopSignalCounter < ExecutableCommands.Count)
            {
                CheckTaskStatuses();
                Thread.Sleep(1000);
            }
        }

        #endregion

        #region Helper Methods

        private void CheckTaskStatuses()
        {
            foreach (Task commandTask in RunningTasks.ToArray())
            {
                if (commandTask.IsCompleted || commandTask.IsCanceled)
                {
                    RunningTasks.Remove(commandTask);
                    StopSignalCounter++;
                }
            }
        }

        private void RunEachCommandInNewThread()
        {
            foreach (IJhCommand command in ExecutableCommands)
            {
                Task commandTask = Task.Factory.StartNew(() => { command.Execute(); });
                RunningTasks.Add(commandTask);
            }
        }

        #endregion

    }
}
