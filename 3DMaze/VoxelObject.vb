Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Input

Public Class VoxelObject
    Inherits SceneObject
    Private Const EXIT_TILE As Integer = 7
    Private Const EXIT_COUNT As Integer = 30

    Private wallVertices As VertexPositionNormalTexture()
    Private exitVertices As VertexPositionNormalTexture()

    Private wallBuffer As VertexBuffer
    Private exitBuffer As VertexBuffer
    Private texture As Texture2D
    Private currentTileset As Integer

    Private canRegen As Boolean

    Public data As Block(,,)
    Public height As Integer
    Public width As Integer
    Public depth As Integer

    Public wallsEffect As BasicEffect
    Public exitEffect As BasicEffect

    Public Sub New(scene As Scene, width As Integer, height As Integer, depth As Integer)
        MyBase.New(scene)
        Me.currentTileset = 5
        Me.canRegen = True

        Me.width = width
        Me.height = height
        Me.depth = depth

        Me.data = New Block(width - 1, depth - 1, height - 1) {}
        Me.wallsEffect = New BasicEffect(scene.GraphicsDevice)
        Me.wallsEffect.DiffuseColor = New Vector3(0, 1, 0.33)

        If Me.texture IsNot Nothing Then
            Me.wallsEffect.TextureEnabled = True
            Me.wallsEffect.Texture = Me.texture
        End If

        Me.wallsEffect.FogColor = New Vector3(0.0F, 0.0F, 0.0F)
        Me.wallsEffect.FogStart = 0.0F
        Me.wallsEffect.FogEnd = 4000.0F
        Me.wallsEffect.FogEnabled = True
        Me.wallsEffect.EnableDefaultLighting()

        ' added effect for exit

        Me.exitEffect = New BasicEffect(scene.GraphicsDevice)
        Me.exitEffect.DiffuseColor = New Vector3(1, 0, 0.33)

        If Me.texture IsNot Nothing Then
            Me.exitEffect.TextureEnabled = True
            Me.exitEffect.Texture = Me.texture
        End If

        Me.exitEffect.FogColor = New Vector3(0.0F, 0.0F, 0.0F)
        Me.exitEffect.FogStart = 0.0F
        Me.exitEffect.FogEnd = 4000.0F
        Me.exitEffect.FogEnabled = True
        Me.exitEffect.EnableDefaultLighting()
        ' end

        For i As Integer = 0 To width - 1
            For j As Integer = 0 To depth - 1
                For k As Integer = 0 To height - 1
                    Dim bloq As New Block() With {.bx = i, .by = j, .bz = k}
                    Me.data(i, j, k) = bloq
                Next
            Next
        Next

        Me.Rebuild()
    End Sub

    Public Sub AddBlockSide(x As Integer, y As Integer, z As Integer, tile As Integer, turn As Single, pitch As Single, normal As Vector3, list As IList(Of VertexPositionNormalTexture))
        Dim vectorArray As Vector3() = New Vector3(5) {}
        Dim pointTri2D As Vector2() = New Vector2(5) {}
        Dim num As Integer = CInt(Math.Truncate(Math.Floor(CDbl(CSng(tile) / 4.0F))))
        Dim num2 As Integer = tile - (num * 4)
        pointTri2D(0) = New Vector2(0.0F, 0.0F)
        pointTri2D(1) = New Vector2(1.0F, 0.0F)
        pointTri2D(2) = New Vector2(1.0F, 1.0F)
        pointTri2D(3) = New Vector2(0.0F, 0.0F)
        pointTri2D(4) = New Vector2(1.0F, 1.0F)
        pointTri2D(5) = New Vector2(0.0F, 1.0F)
        vectorArray(0) = New Vector3(-0.5F, 0.5F, 0.5F)
        vectorArray(1) = New Vector3(0.5F, 0.5F, 0.5F)
        vectorArray(2) = New Vector3(0.5F, -0.5F, 0.5F)
        vectorArray(3) = New Vector3(-0.5F, 0.5F, 0.5F)
        vectorArray(4) = New Vector3(0.5F, -0.5F, 0.5F)
        vectorArray(5) = New Vector3(-0.5F, -0.5F, 0.5F)
        For i As Integer = 0 To 5
            pointTri2D(i) = (pointTri2D(i) / 4.0F) + (New Vector2(CSng(num2), CSng(num)) / 4.0F)
            vectorArray(i) = Vector3.Transform(vectorArray(i), Matrix.CreateRotationX(pitch) * Matrix.CreateRotationY(turn))
            list.Add(New VertexPositionNormalTexture(vectorArray(i) + New Vector3(CSng(x), CSng(y), CSng(z)), normal, pointTri2D(i)))
        Next
    End Sub

    Public Sub AddBlockVertices(x As Integer, y As Integer, z As Integer, tile As Integer, list As IList(Of VertexPositionNormalTexture))

        tile = (tile - 1) * 3
        If Not Me.IsBlock(x, y, z + 1) OrElse IsExit(x, y, z + 1) Then Me.AddBlockSide(x, y, z, tile, 0.0F, 0.0F, Vector3.Backward, list)
        If Not Me.IsBlock(x + 1, y, z) OrElse IsExit(x + 1, y, z) Then Me.AddBlockSide(x, y, z, tile, 1.570796F, 0.0F, Vector3.Right, list)
        If Not Me.IsBlock(x, y, z - 1) OrElse IsExit(x, y, z - 1) Then Me.AddBlockSide(x, y, z, tile, 3.141593F, 0.0F, Vector3.Forward, list)
        If Not Me.IsBlock(x - 1, y, z) OrElse IsExit(x - 1, y, z) Then Me.AddBlockSide(x, y, z, tile, 4.712389F, 0.0F, Vector3.Left, list)
        If Not Me.IsBlock(x, y - 1, z) OrElse IsExit(x, y - 1, z) Then Me.AddBlockSide(x, y, z, tile + 2, 0.0F, 1.570796F, Vector3.Down, list)
        If Not Me.IsBlock(x, y + 1, z) OrElse IsExit(x, y + 1, z) Then Me.AddBlockSide(x, y, z, tile + 1, 0.0F, -1.570796F, Vector3.Up, list)
    End Sub

    Public Sub changeTileset(tile As Integer)
        Dim random As New Random()
        If tile <> Me.currentTileset Then
            Me.currentTileset = tile
            For i As Integer = 0 To Me.width - 1
                For j As Integer = 0 To Me.depth - 1
                    For k As Integer = 0 To Me.height - 1
                        If Me.data(i, j, k).tile > 0 Then
                            If Me.currentTileset < 6 Then
                                Me.data(i, j, k).tile = Me.currentTileset
                            Else
                                Me.data(i, j, k).tile = random.Next(1, 6)
                            End If
                        End If
                    Next
                Next
            Next
            Me.Rebuild()
        End If
    End Sub

    Public Function IsBlock(x As Integer, y As Integer, z As Integer) As Boolean
        If (((x >= 0) AndAlso (y >= 0)) AndAlso ((z >= 0) AndAlso (x < Me.width))) AndAlso ((y < Me.depth) AndAlso (z < Me.height)) Then
            Return (Me.data(x, y, z).tile > 0)
        End If
        Return True
    End Function

    Public Function IsExit(x As Integer, y As Integer, z As Integer) As Boolean
        Return (((x >= 0) AndAlso (y >= 0)) AndAlso ((z >= 0) AndAlso (x < Me.width)) AndAlso ((y < Me.depth) AndAlso (z < Me.height))) AndAlso data(x, y, z).tile = EXIT_TILE
    End Function

    Public Sub Randomize()
        Dim random As New Random()
        Dim empty = New List(Of Vector3)
        For i As Integer = 0 To Me.width - 1
            For j As Integer = 0 To Me.depth - 1
                For k As Integer = 0 To Me.height - 1
                    Me.data(i, j, k).tile = random.[Next](-1, 2)
                    If (((j = 0) OrElse (j = (Me.depth - 1))) OrElse ((i = 0) OrElse (i = (Me.width - 1)))) OrElse ((k = 0) OrElse (k = (Me.height - 1))) Then
                        Me.data(i, j, k).tile = 1
                    End If
                    If Me.data(i, j, k).tile <= 0 Then
                        Me.data(i, j, k).tile = 0
                        If ((i > (Me.width \ 3)) AndAlso (k > (Me.height \ 3))) AndAlso ((i < ((Me.width \ 3) * 2)) AndAlso (k < ((Me.height \ 3) * 2))) Then
                            MyBase.scene.player.position = New Vector3(CSng(i), CSng(j), CSng(k)) * 1000.0F
                        End If
                    Else
                        Me.data(i, j, k).tile = Me.currentTileset
                        empty.Add(New Vector3(i, j, k))
                    End If
                Next
            Next
        Next

        Dim exits = New Integer(EXIT_COUNT) {}
        For i As Integer = 0 To exits.Length - 1
            exits(i) = random.[Next](0, empty.Count)
            For j As Integer = 0 To i - 1
                If exits(i) = exits(j) Then
                    i -= 1
                    Exit For
                End If
            Next
        Next

        For i As Integer = 0 To exits.Length - 1
            Dim coords = empty(exits(i))
            data(coords.X, coords.Y, coords.Z).tile = EXIT_TILE
        Next

        Me.Rebuild()
    End Sub

    Public Sub Rebuild()
        Dim wallList = New List(Of VertexPositionNormalTexture)()
        Dim exitList = New List(Of VertexPositionNormalTexture)()
        For i As Integer = 0 To Me.width - 1
            For k As Integer = 0 To Me.depth - 1
                For m As Integer = 0 To Me.height - 1
                    If Me.data(i, k, m).tile > 0 Then
                        Me.AddBlockVertices(i, k, m, Me.data(i, k, m).tile, If(data(i, k, m).tile = EXIT_TILE, exitList, wallList))
                    End If
                Next
            Next
        Next
        wallVertices = wallList.ToArray()
        If Me.wallVertices.Length > 0 Then
            Me.wallBuffer = New VertexBuffer(MyBase.scene.GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, Me.wallVertices.Length, BufferUsage.[WriteOnly])
            Me.wallBuffer.SetData(Of VertexPositionNormalTexture)(Me.wallVertices)
        End If
        exitVertices = exitList.ToArray()
        If Me.exitVertices.Length > 0 Then
            Me.exitBuffer = New VertexBuffer(MyBase.scene.GraphicsDevice, VertexPositionNormalTexture.VertexDeclaration, exitVertices.Length, BufferUsage.[WriteOnly])
            Me.exitBuffer.SetData(Of VertexPositionNormalTexture)(Me.exitVertices)
        End If
    End Sub

    Public Overrides Sub Update(gameTime As GameTime)
        MyBase.Update(gameTime)
        Dim state As KeyboardState = Keyboard.GetState()
        'If state.IsKeyDown(Keys.F5) Then
        '    Me.changeTileset(1)
        'End If
        'If state.IsKeyDown(Keys.F6) Then
        '    Me.changeTileset(2)
        'End If
        'If state.IsKeyDown(Keys.F11) AndAlso Me.canRegen Then
        '    Me.canRegen = False
        '    Me.Randomize()
        'End If
        'If state.IsKeyUp(Keys.F11) Then
        '    Me.canRegen = True
        'End If
    End Sub

    Public Overrides Sub Draw(gameTime As GameTime)
        MyBase.Draw(gameTime)

        Me.wallsEffect.World = MyBase.scene.world * Matrix.CreateScale(1000.0F)
        Me.wallsEffect.Projection = MyBase.scene.projection
        Me.wallsEffect.View = MyBase.scene.view

        If Me.texture IsNot Nothing Then
            Me.wallsEffect.TextureEnabled = True
            Me.wallsEffect.Texture = Me.texture
        End If

        For Each pass As EffectPass In Me.wallsEffect.CurrentTechnique.Passes
            pass.Apply()
            MyBase.scene.GraphicsDevice.SetVertexBuffer(Me.wallBuffer)
            MyBase.scene.GraphicsDevice.DrawPrimitives(0, 0, Me.wallVertices.Length \ 3)
        Next

        Me.exitEffect.World = MyBase.scene.world * Matrix.CreateScale(1000.0F)
        Me.exitEffect.Projection = MyBase.scene.projection
        Me.exitEffect.View = MyBase.scene.view

        If Me.texture IsNot Nothing Then
            Me.exitEffect.TextureEnabled = True
            Me.exitEffect.Texture = Me.texture
        End If

        For Each pass As EffectPass In Me.exitEffect.CurrentTechnique.Passes
            pass.Apply()
            MyBase.scene.GraphicsDevice.SetVertexBuffer(Me.exitBuffer)
            MyBase.scene.GraphicsDevice.DrawPrimitives(0, 0, Me.exitVertices.Length \ 3)
        Next


    End Sub
End Class
