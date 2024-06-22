Imports System.Numerics
Imports SharpGL
Imports SharpGL.SceneGraph
Imports SharpGLTF.Schema2
Imports System.Threading.Tasks
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO

Public Class Form1
    Private gltfModel As ModelRoot
    Private textures As New Dictionary(Of Integer, UInteger)
    Private enableAntialiasing As Boolean = False

    Private Async Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Await LoadGLTFModelAsync("F:\Old\Assets\lt\BoxTextured.glb")
        ListMaterials()
    End Sub

    Private Async Function LoadGLTFModelAsync(filePath As String) As Task
        gltfModel = Await Task.Run(Function() ModelRoot.Load(filePath))
        LoadTextures()
    End Function

    Private Sub LoadTextures()
        If gltfModel Is Nothing Then Return

        Dim gl As OpenGL = OpenglControl1.OpenGL
        gl.Enable(OpenGL.GL_TEXTURE_2D)

        For Each texture In gltfModel.LogicalTextures
            Dim bitmap = MemoryImageToBitmap(texture.PrimaryImage.Content)

            Dim textureId As UInteger() = {0}
            gl.GenTextures(1, textureId)
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId(0))

            Dim bitmapData = bitmap.LockBits(New Rectangle(0, 0, bitmap.Width, bitmap.Height), Imaging.ImageLockMode.ReadOnly, Imaging.PixelFormat.Format32bppArgb)
            gl.PixelStore(OpenGL.GL_UNPACK_ALIGNMENT, 1)
            gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, OpenGL.GL_RGBA, bitmap.Width, bitmap.Height, 0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, bitmapData.Scan0)
            bitmap.UnlockBits(bitmapData)

            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR)
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR)
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT)
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT)

            textures(texture.LogicalIndex) = textureId(0)
        Next
    End Sub

    Private Function MemoryImageToBitmap(memoryImage As SharpGLTF.Memory.MemoryImage) As Bitmap
        Using ms As New MemoryStream(memoryImage.Content.ToArray())
            Return New Bitmap(ms)
        End Using
    End Function

    Private Sub ListMaterials()
        If gltfModel Is Nothing Then Return

        Dim materials = New HashSet(Of Material)()

        For Each scene In gltfModel.LogicalScenes
            For Each node In scene.VisualChildren
                GatherMaterials(node, materials)
            Next
        Next

        For Each material In materials
            Console.WriteLine("Material found: " & material.Name)
        Next
    End Sub

    Private Sub GatherMaterials(node As Node, materials As HashSet(Of Material))
        If node.Mesh IsNot Nothing Then
            For Each prim In node.Mesh.Primitives
                If prim.Material IsNot Nothing Then
                    materials.Add(prim.Material)
                End If
            Next
        End If

        For Each child In node.VisualChildren
            GatherMaterials(child, materials)
        Next
    End Sub

    Private Sub RenderGLTFModel(gl As OpenGL)
        If gltfModel Is Nothing Then Return

        For Each scene In gltfModel.LogicalScenes
            For Each node In scene.VisualChildren
                RenderNode(gl, node)
            Next
        Next
    End Sub

    Private Sub RenderNode(gl As OpenGL, node As Node)
        gl.PushMatrix()

        Dim transform = node.WorldMatrix
        Dim matrixArray = TransformMatrix(transform)
        gl.MultMatrix(matrixArray)

        ' Render the mesh from node
        If node.Mesh IsNot Nothing Then
            RenderMesh(gl, node.Mesh)
        End If

        For Each child In node.VisualChildren
            RenderNode(gl, child)
        Next

        gl.PopMatrix()
    End Sub

    Private Sub RenderMesh(gl As OpenGL, mesh As Mesh)
        For Each prim In mesh.Primitives
            RenderPrimitive(gl, prim)
        Next
    End Sub

    Private Sub RenderPrimitive(gl As OpenGL, prim As MeshPrimitive)
        Dim positions = prim.VertexAccessors("POSITION").AsVector3Array()
        Dim indices = prim.IndexAccessor.AsIndicesArray()
        Dim normals = prim.VertexAccessors("NORMAL").AsVector3Array()
        Dim texCoords = If(prim.VertexAccessors.ContainsKey("TEXCOORD_0"), prim.VertexAccessors("TEXCOORD_0").AsVector2Array(), Nothing)

        If prim.Material IsNot Nothing Then
            Dim baseColorChannel = prim.Material.FindChannel("BaseColor")
            If baseColorChannel.HasValue Then
                If baseColorChannel.Value.Texture IsNot Nothing Then
                    Dim textureIndex = baseColorChannel.Value.Texture.LogicalIndex
                    If textures.ContainsKey(textureIndex) Then
                        gl.BindTexture(OpenGL.GL_TEXTURE_2D, textures(textureIndex))
                    Else
                        gl.BindTexture(OpenGL.GL_TEXTURE_2D, 0)
                    End If
                Else
                    Dim baseColor = baseColorChannel.Value.Color
                    gl.Color(baseColor.X, baseColor.Y, baseColor.Z, baseColor.W)
                End If
            End If
        End If

        gl.Begin(OpenGL.GL_TRIANGLES)
        For Each index In indices
            If texCoords IsNot Nothing Then
                Dim texCoord = texCoords(index)
                gl.TexCoord(texCoord.X, texCoord.Y)
            End If
            Dim normal = normals(index)
            gl.Normal(normal.X, normal.Y, normal.Z)
            Dim position = positions(index)
            gl.Vertex(position.X, position.Y, position.Z)
        Next
        gl.End()
    End Sub

    Private Function TransformMatrix(matrix As Matrix4x4) As Double()
        Return New Double() {
            matrix.M11, matrix.M12, matrix.M13, matrix.M14,
            matrix.M21, matrix.M22, matrix.M23, matrix.M24,
            matrix.M31, matrix.M32, matrix.M33, matrix.M34,
            matrix.M41, matrix.M42, matrix.M43, matrix.M44
        }
    End Function

    Private Sub OpenglControl1_OpenGLDraw(sender As Object, args As RenderEventArgs) Handles OpenglControl1.OpenGLDraw
        Dim gl As OpenGL = OpenglControl1.OpenGL

        gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT Or OpenGL.GL_DEPTH_BUFFER_BIT)
        gl.LoadIdentity()

        Dim cameraX As Single
        Dim cameraY As Single
        Dim cameraZ As Single

        If Single.TryParse(TextBox1.Text, cameraX) AndAlso Single.TryParse(TextBox2.Text, cameraY) AndAlso Single.TryParse(TextBox3.Text, cameraZ) Then
            gl.LookAt(cameraX, cameraY, cameraZ, 0, 0, 0, 0, 1, 0)
            RenderGLTFModel(gl)
            gl.Flush()
        End If
    End Sub

    Private Sub OpenglControl1_OpenGLInitialized(sender As Object, e As EventArgs) Handles OpenglControl1.OpenGLInitialized
        Dim gl As OpenGL = OpenglControl1.OpenGL
        gl.Enable(OpenGL.GL_TEXTURE_2D)
        gl.Enable(OpenGL.GL_DEPTH_TEST)
        gl.ShadeModel(OpenGL.GL_SMOOTH)
        gl.PolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_FILL)

        If enableAntialiasing Then
            gl.Enable(OpenGL.GL_POLYGON_SMOOTH)
            gl.Enable(OpenGL.GL_BLEND)
            gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA)
        End If
    End Sub

    Private Sub OpenglControl1_Resized(sender As Object, e As EventArgs) Handles OpenglControl1.Resized
        Dim gl As OpenGL = OpenglControl1.OpenGL

        ' Set the projection matrix
        gl.MatrixMode(OpenGL.GL_PROJECTION)
        gl.LoadIdentity()
        gl.Perspective(45.0F, CSng(OpenglControl1.Width) / CSng(OpenglControl1.Height), 0.1F, 100.0F)

        ' Back to modelview
        gl.MatrixMode(OpenGL.GL_MODELVIEW)
        gl.LoadIdentity()
    End Sub
End Class
