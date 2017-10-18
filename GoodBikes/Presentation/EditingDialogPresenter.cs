using System;
using System.Windows.Forms;
using View;

namespace GoodBikes.Presentation
{
    public class EditingDialogPresenter
    {
        private readonly EditingDialog _target;
        private readonly EditingControlPresenter _editingControlPresenter;
        private readonly bool _firstEdit;

        public EditingDialogPresenter(EditingDialog target, bool firstEdit)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            _target = target;
            _firstEdit = firstEdit;
            _editingControlPresenter = new EditingControlPresenter(Target.EditingControl, _firstEdit);
            Target.ResetButton.Click += ResetButton_Click;
            Target.CancelButton_.Click += CancelButton_Click;
            Application.Idle += Application_Idle;
        }

        public EditingDialog Target
        {
            get { return _target; }
        }

        private EditingControlPresenter EditingControlPresenter
        {
            get { return _editingControlPresenter; }
        }

        public void SetEditableObject(object editingObject)
        {
            EditingControlPresenter.SetEditableObject(editingObject);
            Target.ResetButton.Enabled = !EditingControlPresenter.HasError;
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            Target.OkButton.Enabled = !EditingControlPresenter.HasError;
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            EditingControlPresenter.ResetEditingObject();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            EditingControlPresenter.ResetEditingObject();
        }
    }
}
