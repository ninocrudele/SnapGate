#region License
//-----------------------------------------------------------------------
// <copyright file="ShowServiceFormSettingCreation.cs" company="Antonino Crudele">
//   Copyright (c) Antonino Crudele. All Rights Reserved.
// </copyright>
// <license>
//   This work is registered with the UK Copyright Service.
//   Registration No:284695248
//   Licensed under the Reciprocal Public License 1.5 (RPL1.5) 
//   See License.txt in the project root for license information.
// </license>
//-----------------------------------------------------------------------
#endregion

namespace Framework.Forms
{
    using System;
    using System.ServiceProcess;
    using System.Windows.Forms;

    using Base;

    /// <summary>
    /// Form to set the service settings.
    /// </summary>
    public partial class ShowServiceFormSettingCreation : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowServiceFormSettingCreation"/> class.
        /// </summary>
        public ShowServiceFormSettingCreation()
        {
            this.InitializeComponent();
        } // Constructor

        /// <summary>
        /// Enables the controls.
        /// </summary>
        /// <param name="enabled"><c>true</c> if the controls should enabled, <c>false</c> otherwise.</param>
        internal void EnableControls(bool enabled)
        {
            this.comboBoxAccount.Enabled = enabled;
            this.textBoxPassword.Enabled = enabled;
            this.textBoxUsername.Enabled = enabled;
            this.textBoxDescription.Enabled = enabled;
            this.textBoxDisplayName.Enabled = enabled;
            this.textBoxWindowsNTServiceName.Enabled = enabled;
        } // EnableControls

        /// <summary>
        /// Handles the Load event of the ShowServiceFormSettingCreation control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ShowServiceFormSettingCreationLoad(object sender, EventArgs e)
        {
            this.textBoxRESTEndpoint.Text = Configuration.WebApiEndPoint();
            this.comboBoxAccount.DataSource = Enum.GetValues(typeof(ServiceAccount));

            this.comboBoxAccount.SelectedText = Configuration.WindowsNTServiceAccountType().ToString();
            this.textBoxPassword.Text = Configuration.WindowsNTServicePassword();
            this.textBoxUsername.Text = Configuration.WindowsNTServiceUsername();
            this.textBoxDescription.Text = Configuration.WindowsNTServiceDescription();
            this.textBoxDisplayName.Text = Configuration.WindowsNTServiceDisplayName();
            this.textBoxWindowsNTServiceName.Text = Configuration.WindowsNTServiceName();
        } // ShowServiceFormSettingCreationLoad

        /// <summary>
        /// Handles the CheckedChanged event of the radioButtonFullAffinity control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void RadioButtonFullAffinityCheckedChanged(object sender, EventArgs e)
        {
            this.EnableControls(!this.radioButtonFullAffinity.Enabled);
        } // RadioButtonFullAffinityCheckedChanged

        /// <summary>
        /// Handles the CheckedChanged event of the radioButtonCustomAffinity control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void RadioButtonCustomAffinityCheckedChanged(object sender, EventArgs e)
        {
            this.EnableControls(this.radioButtonFullAffinity.Enabled);
        } // RadioButtonCustomAffinityCheckedChanged

        /// <summary>
        /// Handles the Click event of the buttonGo control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ButtonGoClick(object sender, EventArgs e)
        {
            Configuration.configurationStorage.WebApiEndPoint = this.textBoxRESTEndpoint.Text;

            if (this.radioButtonCustomAffinity.Checked)
            {
                ServiceAccount serviceAccount;
                Enum.TryParse(this.comboBoxAccount.SelectedValue.ToString(), out serviceAccount);

                NTSharedProperties.Account = serviceAccount;
                NTSharedProperties.Password = this.textBoxPassword.Text;
                NTSharedProperties.Username = this.textBoxUsername.Text;
                NTSharedProperties.Description = this.textBoxDescription.Text;
                NTSharedProperties.DisplayName = this.textBoxDisplayName.Text;
                NTSharedProperties.WindowsNTServiceName = this.textBoxWindowsNTServiceName.Text;
            }

            if (this.radioButtonFullAffinity.Checked)
            {
                ServiceAccount serviceAccount;
                Enum.TryParse(this.comboBoxAccount.SelectedValue.ToString(), out serviceAccount);

                NTSharedProperties.Account = Configuration.WindowsNTServiceAccountType();
                NTSharedProperties.Password = Configuration.WindowsNTServicePassword();
                NTSharedProperties.Username = Configuration.WindowsNTServiceUsername();
                NTSharedProperties.Description = Configuration.WindowsNTServiceDescription();
                NTSharedProperties.DisplayName = Configuration.WindowsNTServiceDisplayName();
                NTSharedProperties.WindowsNTServiceName = Configuration.WindowsNTServiceName();
            }

            ServiceFormSettingsCreationParameters.StartupMode = StartupModeEnum.NT;

            switch (ServiceFormSettingsCreationParameters.CreationMode)
            {
                case CreationModeEnum.Install:
                    ServiceFormSettingsCreationParameters.CreationMode = CreationModeEnum.Install;
                    break;
                case CreationModeEnum.InstallAndStart:
                    ServiceFormSettingsCreationParameters.CreationMode = CreationModeEnum.InstallAndStart;
                    break;
                case CreationModeEnum.Uninstall:
                    ServiceFormSettingsCreationParameters.CreationMode = CreationModeEnum.Uninstall;
                    break;
                default:
                    ServiceFormSettingsCreationParameters.CreationMode = CreationModeEnum.Install;
                    break;
            }

            this.Close();
        } // ButtonGoClick




        /// <summary>
        /// Handles the Click event of the buttonCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ButtonCancelClick(object sender, EventArgs e)
        {
            ServiceFormSettingsCreationParameters.StartupMode = StartupModeEnum.Close;
            this.Close();
        } // ButtonCancelClick

        /// <summary>
        /// Handles the CheckedChanged event of the radioButtonInstall control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void RadioButtonInstallCheckedChanged(object sender, EventArgs e)
        {
            ServiceFormSettingsCreationParameters.CreationMode = CreationModeEnum.Install;
        } // RadioButtonInstallCheckedChanged

        /// <summary>
        /// Handles the CheckedChanged event of the radioButtonInstallAndRun control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void RadioButtonInstallAndRunCheckedChanged(object sender, EventArgs e)
        {
            ServiceFormSettingsCreationParameters.CreationMode = CreationModeEnum.InstallAndStart;
        } // RadioButtonInstallAndRunCheckedChanged

        /// <summary>
        /// Handles the CheckedChanged event of the radioButtonUninstall control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void RadioButtonUninstallCheckedChanged(object sender, EventArgs e)
        {
            ServiceFormSettingsCreationParameters.CreationMode = CreationModeEnum.Uninstall;
        } // RadioButtonUninstallCheckedChanged

        private void buttonRunConsole_Click(object sender, EventArgs e)
        {
            Configuration.configurationStorage.WebApiEndPoint = this.textBoxRESTEndpoint.Text;

            ServiceFormSettingsCreationParameters.StartupMode = StartupModeEnum.Console;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.groupBoxAffinity.Enabled = true;
            this.buttonClone.Enabled = true;
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {

        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {

        }
    } // ShowServiceFormSettingCreation
} // namespace