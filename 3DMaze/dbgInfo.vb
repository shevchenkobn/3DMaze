﻿Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports System.Diagnostics

Public Class dbgInfo
    Inherits DrawableGameComponent
    Public Level As Integer = 1
    Private _fps As Integer
    Private _time As Double
    Public Shared debug As String = "W, S, A, D: Movement" & vbLf & "Space: Jump" & vbLf & "Shift: Run" & vbLf & "RMB: Zoom-in" & vbLf & "ESC: quit"
    Private font As SpriteFont
    Private fps As Integer
    Private spriteBatch As SpriteBatch

    Public Sub New(game As Game)
        MyBase.New(game)
    End Sub

    Public Overrides Sub Draw(gameTime As GameTime)
        Me._fps += 1
        Me._time += gameTime.ElapsedGameTime.TotalSeconds
        While Me._time > 1.0
            Me.fps = Me._fps
            Me._fps = 0
            Me._time -= 1
        End While
        MyBase.Draw(gameTime)
        Me.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.Default, RasterizerState.CullCounterClockwise)
        Dim str As String = String.Concat(New Object() {"Level: " & Level, vbLf, vbLf, vbLf, vbLf, vbLf, "FPS: ", Me.fps, vbLf, debug})
        'System.Diagnostics.Debug.WriteLine(Level)
        Me.spriteBatch.DrawString(Me.font, str, Vector2.One, Color.Black)
        Me.spriteBatch.DrawString(Me.font, str, Vector2.Zero, Color.Red)
        Me.spriteBatch.End()
    End Sub

    Public Overrides Sub Initialize()
        MyBase.Initialize()
        Me.spriteBatch = New SpriteBatch(MyBase.GraphicsDevice)
    End Sub

    Protected Overrides Sub LoadContent()
        MyBase.LoadContent()
        Me.font = MyBase.Game.Content.Load(Of SpriteFont)("dbg")
    End Sub
End Class
