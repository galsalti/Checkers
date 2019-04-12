using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CheckersUI
{
    internal class FormLogin : Form
    {
        private bool m_LoginSuccessful = false;
        private string m_FirstPlayerName;
        private string m_SecondPlayerName;
        private bool m_VsComputer;
        private ushort m_BoardSize;
        private Label m_LabelBoardSize = new Label();
        private Label m_LabelPlayers = new Label();
        private Label m_LabelPlayer1 = new Label();
        private Label m_LabelPlayer2 = new Label();
        private Label m_Label6x6 = new Label();
        private Label m_Label8x8 = new Label();
        private Label m_Label10x10 = new Label();
        private RadioButton m_RadioButton6x6 = new RadioButton();
        private RadioButton m_RadioButton8x8 = new RadioButton();
        private RadioButton m_RadioButton10x10 = new RadioButton();
        private CheckBox m_CheckBoxEnablePlayer2 = new CheckBox();
        private TextBox m_TextBoxPlayer1Name = new TextBox();
        private TextBox m_TextBoxPlayer2Name = new TextBox();
        private Button m_ButtonDone = new Button();

        public FormLogin()
        {
            this.Size = new Size(280, 250);
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Game Settings";
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            initControls();
        }

        private void initControls()
        {
            m_LabelBoardSize.Text = "Board Size:";
            m_LabelBoardSize.Location = new Point(10, 20);

            m_Label6x6.Text = "6x6";
            m_Label6x6.Location = new Point(40, 50);
            m_Label6x6.AutoSize = true;

            m_Label8x8.Text = "8x8";
            m_Label8x8.Location = new Point(120, 50);
            m_Label8x8.AutoSize = true;


            m_Label10x10.Text = "10x10";
            m_Label10x10.Location = new Point(200, 50);
            m_Label8x8.AutoSize = true;

            m_LabelPlayers.Text = "Players:";
            m_LabelPlayers.Location = new Point(10, 80);

            m_LabelPlayer1.Text = "Player 1:";
            m_LabelPlayer1.Location = new Point(30, 110);
            m_LabelPlayer1.AutoSize = true;

            m_LabelPlayer2.Text = "Player 2:";
            m_LabelPlayer2.Location = new Point(50, 140);
            m_LabelPlayer2.AutoSize = true;

            int currentYLocation = m_Label6x6.Top + ((m_Label6x6.Height - m_RadioButton6x6.Height) / 2);
            m_RadioButton6x6.AutoSize = true;
            m_RadioButton6x6.Location = new Point(m_Label6x6.Left - 15, currentYLocation);

            currentYLocation = m_Label10x10.Top + ((m_Label8x8.Height - m_RadioButton8x8.Height) / 2);
            m_RadioButton8x8.AutoSize = true;
            m_RadioButton8x8.Location = new Point(m_Label8x8.Left - 15, currentYLocation);

            currentYLocation = m_Label10x10.Top + ((m_Label10x10.Height - m_RadioButton10x10.Height) / 2);
            m_RadioButton10x10.AutoSize = true;
            m_RadioButton10x10.Location = new Point(m_Label10x10.Left - 15, currentYLocation);

            currentYLocation = m_LabelPlayer2.Top + ((m_LabelPlayer2.Height - m_LabelPlayer2.Height) / 2);
            m_CheckBoxEnablePlayer2.AutoSize = true;
            m_CheckBoxEnablePlayer2.Location = new Point(m_LabelPlayer2.Left - 15, currentYLocation);

            currentYLocation = m_LabelPlayer1.Top + ((m_LabelPlayer1.Height - m_LabelPlayer1.Height) / 2);
            m_TextBoxPlayer1Name.AutoSize = true;
            m_TextBoxPlayer1Name.Location = new Point(m_LabelPlayer1.Right + 10, currentYLocation);

            currentYLocation = m_LabelPlayer2.Top + ((m_LabelPlayer2.Height - m_LabelPlayer2.Height) / 2);
            m_TextBoxPlayer2Name.AutoSize = true;
            m_TextBoxPlayer2Name.Text = "[Computer]";
            m_TextBoxPlayer2Name.ReadOnly = true;
            m_TextBoxPlayer2Name.Location = new Point(m_LabelPlayer1.Right + 10, currentYLocation);

            m_ButtonDone.Location = new Point(170, m_TextBoxPlayer2Name.Bottom + 15);
            m_ButtonDone.Text = "Done";
            m_ButtonDone.AutoSize = true;

            this.Controls.AddRange(new Control[] { m_Label10x10, m_Label6x6, m_Label8x8, m_LabelBoardSize, m_LabelPlayer1, m_LabelPlayer2, m_LabelPlayers, m_RadioButton6x6,
                m_RadioButton8x8, m_RadioButton10x10, m_CheckBoxEnablePlayer2, m_TextBoxPlayer1Name, m_TextBoxPlayer2Name, m_ButtonDone});

            m_ButtonDone.Click += new EventHandler(this.m_ButtonDone_Click);
            m_CheckBoxEnablePlayer2.CheckedChanged += new EventHandler(this.m_CheckBoxEnablePlayer2_CheckedChange);
        }

        internal bool LoginSuccessful
        {
            get { return m_LoginSuccessful; }
        }

        internal bool VsComputer
        {
            get { return m_VsComputer; }
        }
        internal ushort BoardSize
        {
            get { return m_BoardSize; }
        }

        internal string FirstPlayerName
        {
            get { return m_FirstPlayerName; }
        }

        internal string SecondPlayerName
        {
            get { return m_SecondPlayerName; }
        }

        private void m_CheckBoxEnablePlayer2_CheckedChange(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
            {
                m_TextBoxPlayer2Name.ReadOnly = false;
            }
            else
            {
                m_TextBoxPlayer2Name.Text = "[Computer]";
                m_TextBoxPlayer2Name.ReadOnly = true;
            }
        }

        private void m_ButtonDone_Click(object sender, EventArgs e)
        {
            if (!checkIfLoginInformationIsLegal())
            {
                if (MessageBox.Show(
                    @"Illegal information!
Please make sure that:
(1) Name is up to 20 characters, without spaces.
(2) Board size is chosen",
                    "Login",
                    MessageBoxButtons.RetryCancel,
                    MessageBoxIcon.Error) == DialogResult.Cancel)
                {
                    this.Close();
                }
            }
            else
            {
                this.Close();
                m_LoginSuccessful = true;
                m_FirstPlayerName = m_TextBoxPlayer1Name.Text;
                m_VsComputer = !m_CheckBoxEnablePlayer2.Checked;
                m_SecondPlayerName = m_TextBoxPlayer2Name.Text;
                m_BoardSize = convertRadioButtonToBoardSize();
            }
        }

        private ushort convertRadioButtonToBoardSize()
        {
            ushort boardSize;

            if (m_RadioButton10x10.Checked)
            {
                boardSize = 10;
            }
            else if (m_RadioButton8x8.Checked)
            {
                boardSize = 8;
            }
            else
            {
                boardSize = 6;
            }

            return boardSize;
        }

        private bool checkIfLoginInformationIsLegal()
        {
            bool legalInformation = checkIfLegalBoardSize() && checkIfLegalName(m_TextBoxPlayer1Name.Text);
            if (m_CheckBoxEnablePlayer2.Checked)
            {
                legalInformation = legalInformation && checkIfLegalName(m_TextBoxPlayer2Name.Text);
            }

            return legalInformation;
        }

        private bool checkIfLegalBoardSize()
        {
            bool legalBoardSize = m_RadioButton10x10.Checked || m_RadioButton8x8.Checked
                || m_RadioButton6x6.Checked;

            return legalBoardSize;
        }

        private bool checkIfLegalName(string i_InputName)
        {
            bool isLegalName = true && i_InputName.Length <= 20 && i_InputName.Length > 0;
            for (int currentChar = 0; currentChar < i_InputName.Length && isLegalName; currentChar++)
            {
                if (char.IsWhiteSpace(i_InputName[currentChar]))
                {
                    isLegalName = false;
                }
            }

            return isLegalName;
        }
    }
}
