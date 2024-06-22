<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        OpenglControl1 = New SharpGL.OpenGLControl()
        TextBox1 = New TextBox()
        TextBox2 = New TextBox()
        TextBox3 = New TextBox()
        CType(OpenglControl1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' OpenglControl1
        ' 
        OpenglControl1.Dock = DockStyle.Fill
        OpenglControl1.DrawFPS = True
        OpenglControl1.FrameRate = 30
        OpenglControl1.Location = New Point(0, 0)
        OpenglControl1.Margin = New Padding(4, 3, 4, 3)
        OpenglControl1.Name = "OpenglControl1"
        OpenglControl1.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1
        OpenglControl1.RenderContextType = SharpGL.RenderContextType.DIBSection
        OpenglControl1.RenderTrigger = SharpGL.RenderTrigger.TimerBased
        OpenglControl1.Size = New Size(800, 450)
        OpenglControl1.TabIndex = 0
        ' 
        ' TextBox1
        ' 
        TextBox1.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        TextBox1.Location = New Point(688, 362)
        TextBox1.Name = "TextBox1"
        TextBox1.Size = New Size(100, 23)
        TextBox1.TabIndex = 1
        TextBox1.Text = "10"
        ' 
        ' TextBox2
        ' 
        TextBox2.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        TextBox2.Location = New Point(688, 391)
        TextBox2.Name = "TextBox2"
        TextBox2.Size = New Size(100, 23)
        TextBox2.TabIndex = 2
        TextBox2.Text = "10"
        ' 
        ' TextBox3
        ' 
        TextBox3.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        TextBox3.Location = New Point(688, 420)
        TextBox3.Name = "TextBox3"
        TextBox3.Size = New Size(100, 23)
        TextBox3.TabIndex = 3
        TextBox3.Text = "10"
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 450)
        Controls.Add(TextBox3)
        Controls.Add(TextBox2)
        Controls.Add(TextBox1)
        Controls.Add(OpenglControl1)
        Name = "Form1"
        Text = "Form1"
        CType(OpenglControl1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents OpenglControl1 As SharpGL.OpenGLControl
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents TextBox2 As TextBox
    Friend WithEvents TextBox3 As TextBox

End Class