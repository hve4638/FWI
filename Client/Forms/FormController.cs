using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWIClient
{
    public class FormController
    {
        Form? form;
        public bool IsAlive => form?.IsDisposed ?? false;

        public void RunSync(Form form)
        {
            Application.Run(form);
        }

        public Task Run(Form form)
        {
            Close();
            this.form = form;

            var task = new Task(() => Application.Run(form));
            task.Start();

            return task;
        }

        public void Close()
        {
            if (IsAlive)
            {
                form?.Close();
                form = null;
            }
        }
    }
}
