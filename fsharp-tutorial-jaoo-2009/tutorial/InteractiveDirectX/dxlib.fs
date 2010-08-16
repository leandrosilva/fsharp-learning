//----------------------------------------------------------------------------
// The Famous DirectX Demo - library code
//
// Copyright (c) Microsoft Corporation 2005-2008.
//
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 
//----------------------------------------------------------------------------


module Sample.DirectX

open System
open System.Drawing
open System.Threading  
open System.Windows.Forms
open Microsoft.DirectX
open Microsoft.DirectX.Direct3D

//----------------------------------------------------------------------------
// common
//----------------------------------------------------------------------------

module MathOps = 
    type time = float
    let now () = (float Environment.TickCount / 1000.0) 
    let sqr (x:float) = x * x
    let pi = Math.PI 

/// Vectors: origin, basis and operations 
module VectorOps =

    open MathOps

    let O   = Vector3( 0.0f, 0.0f, 0.0f) 
    let X1  = Vector3( 1.0f, 0.0f, 0.0f) 
    let Y1  = Vector3( 0.0f, 1.0f, 0.0f)
    let Z1  = Vector3( 0.0f, 0.0f, 1.0f)

    let cross     u v = Vector3.Cross(u,v)
    let normalize u   = Vector3.Normalize(u)
    let scale     k u = Vector3.Scale(u,k)
    let planeProject n v = v - scale (Vector3.Dot(n,v)) n // n is plane normal 
    let transformAt v m = Matrix.Translation(v) * m * Matrix.Translation(-v)  
    let singlev (x,y,z) = Vector3(single x,single y,single z)


open MathOps
open VectorOps


//----------------------------------------------------------------------------
// DirectX form
//
// Create an enclosing form which owns its own painting.
//----------------------------------------------------------------------------

type SmoothForm() as x = 
    inherit Form()
    do x.SetStyle(ControlStyles.AllPaintingInWmPaint ||| ControlStyles.Opaque ||| ControlStyles.UserPaint, true);


//----------------------------------------------------------------------------
// Color Interpolation
//----------------------------------------------------------------------------

module Vertex =     

    let Colored (c:Color) v = CustomVertex.PositionNormalColored(v,Z1,c.ToArgb())

    let White v = Colored Color.White v

    let Interpolated x v =
        let r,g,b =
            if x < 0.0f then (1.0f,0.0f,0.0f)
            elif x < 0.5f then 
               let z = 2.0f * x       
               1.0f-z,z,0.0f 
            elif x < 1.0f then
               let z = 2.0f * x - 1.0f 
               0.0f,1.0f-z,z   
            else
               0.0f,0.0f,1.0f
        let byte x = int (x * 255.0f) 
        let color = Color.FromArgb(0,byte r,byte g,byte b)
        Colored color v

//----------------------------------------------------------------------------
// Mouse events
//----------------------------------------------------------------------------

type System.Windows.Forms.Form with 
    /// Track the movement of the mouse across a form
    member form.MousePositions = 
        let event = new Event<_>()
        let lastArgs = ref None
        form.MouseDown.Add(fun args -> lastArgs := Some args)
        form.MouseUp.Add  (fun args -> lastArgs := None)
        form.MouseMove.Add(fun args -> 
            match !lastArgs with
            | Some last -> event.Trigger(last,args); lastArgs := Some args
            | None -> ());
        event.Publish
                                 
    
/// The yaw, pitch, roll, Focus, Zoom settings for a view
type View = 
    { YawPitchRoll: Matrix;
      Focus       : Vector3;
      Zoom        : float }
    with
        member view.AdjustZoom(dx,dy) = 
            { view with Zoom = view.Zoom * exp (float dy / 100.0) } 

        member view.AdjustYawPitchRoll(dx,dy) = 
            let rx = float32 dx / 20.0f
            let ry = float32 dy / 20.0f    
            let m = Matrix.RotationYawPitchRoll(ry,0.0f,rx)    // Rotate 
            let m = transformAt ((X1 + Y1 + Z1) *  -0.5f ) m // at centre Vertex.White 
            { view with YawPitchRoll = m * view.YawPitchRoll }

        member view.AdjustFocus(dx,dy) = 
            let dv = Y1 * (float32 (-dx) / 50.0f)  + Z1 * (float32 dy / 50.0f) 
            { view with Focus = view.Focus + dv } // Move Focus 
    end


/// The (X,Y) mesh used to draw a scene
type BaseMesh(X,Y) = 
    static member Create(gx,gy,m,n) =
        let X = Array2D.init n m (fun i j -> gx n i)
        let Y = Array2D.init n m (fun i j -> gy m j)
        BaseMesh(X,Y)
        
    static member Grid(m,n) = 
        let normi n i = float32 i / float32 (n-1)
        BaseMesh.Create(normi,normi,m,n)

    member x.Dimensions = Array2D.length1 X, Array2D.length2 Y

    member x.Item with get(i,j  ) = X.[i,j], Y.[i,j]

    member x.ToIndex(i,j) = let m,n = x.Dimensions in i + j * n 

    member x.FromIndex(k) = let m,n = x.Dimensions in k%n,k/n   


type VertexBuffer<'primType>(size,device,usage,pool) = 
    let format = 
        match (typeof<'primType>) with 
        | ty when ty = (typeof<CustomVertex.PositionColored>)         -> CustomVertex.PositionColored.Format
        | ty when ty = (typeof<CustomVertex.PositionColoredTextured>) -> CustomVertex.PositionColoredTextured.Format
        | ty when ty = (typeof<CustomVertex.PositionOnly>)            -> CustomVertex.PositionOnly.Format
        | ty when ty = (typeof<CustomVertex.PositionNormalColored>)   -> CustomVertex.PositionNormalColored.Format
        | ty when ty = (typeof<CustomVertex.PositionNormalTextured>)  -> CustomVertex.PositionNormalTextured.Format
        | _ -> failwith "unknown format type" 
    let vertexBuffer =  new VertexBuffer((typeof<'primType>), size, device, usage, format, pool) 

    member x.VertexBuffer = vertexBuffer
    member x.SetData(pts :'primType[]) = vertexBuffer.SetData(pts,0,LockFlags.None)

    member x.DrawPrimitive primitive nPrim =
        device.SetStreamSource(streamNumber=0,streamData=vertexBuffer,offsetInBytes=0);
        device.VertexFormat <- format;
        device.DrawPrimitives(primitive,0,nPrim)

    interface IDisposable with 
        member x.Dispose() = vertexBuffer.Dispose()

    static member OfPoints(pts : 'primType[], device) =
        let vertexBuffer = new VertexBuffer<'primType>
                                    (pts.Length, // number pts 
                                     device,     
                                     Usage.None,
                                     // Pool.Managed resources survive device loss 
                                     Pool.Managed) 
        vertexBuffer.SetData(pts);
        vertexBuffer

              
/// The renderer and scene display
type DirectXRenderer(control:Control) = 

    let draw = new Event<_>()
    let mutable view : View option = None
    let mutable surface : (float32 -> float32 -> float32 -> float32) option = None
    let mutable mesh : BaseMesh = BaseMesh.Grid(20, 20)
    let pparams  = 
        PresentParameters(Windowed=true, 
                          SwapEffect=SwapEffect.Discard,
                          // Turn on a Depth stencil
                          EnableAutoDepthStencil=true, 
                          // And the stencil format
                          AutoDepthStencilFormat=DepthFormat.D16)  
    let device =
      new Device(0, DeviceType.Hardware, control,
                    CreateFlags.SoftwareVertexProcessing, 
                    [| pparams |]) 

    /// Reset the device if it is rudely taken from us
    let rec checkResetThen f = 
        if device.CheckCooperativeLevel() 
        then f()
        else 
            let mutable res = 0
            let ok = device.CheckCooperativeLevel(&res)
            if not ok && res = int ResultCode.DeviceNotReset then 
                device.Reset([|pparams|]);
                checkResetThen f

    let checkVisibleThen f = if control.Visible then f()
        
    let clearScene (color:Color) (device:Device) =
        device.Clear(ClearFlags.ZBuffer ||| ClearFlags.Target,color, 1.0f, 0)

    /// If device available and not hidden, do the required device actions 
    let doRender () = 
        checkResetThen (fun () -> 
            checkVisibleThen (fun () -> 
                device.BeginScene();
                clearScene Color.Black device;
                draw.Trigger(now());
                device.EndScene();
                try device.Present() with _ -> ()))
        
    // doInitialize: initialise device properties and invalidate to trigger redraw       
    let doInitialize() =
        device.RenderState.ZBufferEnable <- true;
        device.RenderState.Ambient <- Drawing.Color.White;
        control.Invalidate()
      
    // Render loop and initial condition
    do device.DeviceReset.Add(fun _ -> doInitialize())
    do doInitialize()
    do control.Paint.Add(fun _ -> doRender(); control.Invalidate())

    // publish
    member x.Device = device
    member x.DrawScene = draw.Publish

    member x.DrawTriangeStrip (ptsA : CustomVertex.PositionNormalColored[]) =
        using (VertexBuffer.OfPoints(ptsA,device)) (fun vb -> 
            vb.DrawPrimitive PrimitiveType.TriangleStrip (ptsA.Length-2))

    member x.DrawLines (ptsA : CustomVertex.PositionNormalColored[]) =
        using (VertexBuffer<_>.OfPoints(ptsA,device)) (fun vb -> 
            vb.DrawPrimitive PrimitiveType.LineList (ptsA.Length/2))

    member x.DrawSurface (mesh:BaseMesh) f =
        let nRows,nCols = mesh.Dimensions
        let data = Array.init (nCols*nRows) (fun k ->
                                       let i,j = mesh.FromIndex(k)
                                       let x,y = mesh.[i,j]
                                       let z = f x y  
                                       (x,y,z))  



        let blendPlace (i,j) =
            let k = mesh.ToIndex(i,j)
            let x,y,z = data.[k]
            Vertex.Interpolated z (X1*x + Y1*y + Z1*z)

        let colorPlace c (i,j) =
            let k = mesh.ToIndex(i,j)
            let x,y,z = data.[k]
            Vertex.Colored c (X1*x + Y1*y + Z1*z)

        let triangleRows = [| for j in 0 .. (nRows-2) 
                                -> [| for i in 0 .. (2*nCols-1) 
                                       -> if i%2 = 0 then (i/2,j) else (i/2,j+1) |] |]

        for row in triangleRows do 
            let strip = Array.map blendPlace row
            x.DrawTriangeStrip strip
            
        let lines = seq { for i in 0 .. nCols-2 do
                          for j in 0 .. nRows-2 do
                          yield (i,j) 
                          yield (i+1,j)
                          yield (i,j)
                          yield (i,j+1) }
                      
        let lines = lines |> Seq.map (colorPlace Color.Black)  |> Array.ofSeq
        x.DrawLines lines

    member x.DrawCubeAxis() =

        let planeN = 6  // number of division on XY plane grid 
        let planePts = 
            [| for i in 0 .. planeN do 
               let k = float32 i / float32 planeN 
               for p in  [| Y1*k; X1 + Y1*k;         // Line k.Y  to    X + k.Y 
                            X1*k; Y1 + X1*k; |]      // Line k.X  to  k.X +   Y 
               -> Vertex.Colored Color.Gray p |]

        let boxPts =
            Array.append
              (Array.map Vertex.White 
                 [| O ;O  + Z1;
                    O ;X1;
                    X1 ;X1 + Y1; |])
              (Array.map (Vertex.Colored Color.Gray)
                 [| Y1      ; Y1      + Z1;
                    X1 + Y1 ; X1 + Y1 + Z1;
                    O + Z1 ; Y1      + Z1;
                    Y1 + Z1 ; X1 + Y1 + Z1; |])

        device.RenderState.CullMode <- Cull.None;
        x.DrawLines planePts;
        x.DrawLines boxPts 

    member x.DrawPlaneArrow n p dir = 
        let dir2 = normalize (cross n dir)
        x.DrawLines 
          (Array.map 
              Vertex.White 
              [| p + dir * 0.15f ; p;
                 p + dir * 0.15f ; p + dir * 0.10f  + dir2 * 0.02f ;
                 p + dir * 0.15f ; p + dir * 0.10f  - dir2 * 0.02f |]) 


    /// The view parameters for the scene
    member x.View 
     with get() = 
        match view with 
        | None -> 
           { YawPitchRoll   = Matrix.RotationYawPitchRoll(0.0f,0.0f,0.0f);
             Focus          = scale 0.5f (X1 + Y1 + Z1);
             Zoom           = 4.0 }
        | Some v -> v
     and set v = view <- Some v
    /// The surface for the scene
    member x.Surface 
     with get() =  
        match surface with 
        | None -> (fun _ _ _ -> 1.0) 
        | Some f -> (fun (t:float) (x:float) (y:float) -> f (float32 t) (float32 x) (float32 y) |> float)
     and set f = surface <- Some (fun t x y -> (f (float t) (float x) (float y) : float) |> float32)

    /// The mesh used to draw the surface for the scene
    member x.Mesh with set v = mesh <- v

    member x.SetView view =
        let eye = (X1 + Y1 + Z1) * 2.0f - X1 * single view.Zoom
        device.Transform.View <- Matrix.Invert(view.YawPitchRoll) * Matrix.LookAtLH(eye,view.Focus,Z1);
        device.Transform.Projection <-
            Matrix.PerspectiveFovLH(fieldOfViewY=single (Math.PI / 8.0),  
                                    aspectRatio =1.0f,                    
                                    znearPlane  =0.1f,                    
                                    zfarPlane   =100.0f);
        device.Transform.World <- Matrix.Identity

    member x.SetupLights() =
        let mutable material = new Direct3D.Material(DiffuseColor=ColorValue.FromColor(Color.White),
                                                     AmbientColor=ColorValue.FromColor(Color.White))
        device.Material <- material
        device.RenderState.Lighting <- true
        device.Lights.[0].Type      <- LightType.Directional
        device.Lights.[0].Diffuse   <- System.Drawing.Color.White
        device.Lights.[0].Direction <- Vector3(0.0f,0.0f,-1.0f)
        device.Lights.[0].Enabled   <- true
        device.RenderState.Ambient <- System.Drawing.Color.FromArgb(0x101010)
        match view with 
        | None -> ()
        | Some v -> x.SetView v
        let t = now()
        match surface with 
        | Some s -> x.DrawSurface mesh (s (float32 t))
        | _ -> ()

    member renderer.DrawArrows  (n,p, p0, pV) = 
        // vertical line 
        renderer.DrawLines (Array.map (Vertex.Colored Color.Gray) [| p0;p |])
        // velocity arrow on floor 
        renderer.DrawPlaneArrow Z1 p0 pV      
        // normal arrow at point     
        renderer.DrawPlaneArrow (cross n X1) p  (scale 0.8f n)
      
    member renderer.DrawTorus(n,p,radiusA:float,radiusB:float) = 
    // Now draw the mesh
        renderer.Device.Transform.World <- 
            (let m = Matrix.LookAtLH(p + scale (float32 radiusB) n,p+n,X1) 
             Matrix.Invert(m))
        use mesh = Mesh.Torus(renderer.Device,float32 radiusB,float32 radiusA,20,20)
        mesh.ComputeNormals()
        mesh.DrawSubset(0)
          
        renderer.Device.Transform.World <- Matrix.Identity

type surface = float -> float -> float

type Agent<'T> = MailboxProcessor<'T>

/// The renderer and scene display
type SimulationState<'T>() = 

    let mutable objects : 'T list = []

    member x.Objects with set (v:'T list) = objects <- v //[ for x in (v :?> System.Collections.IEnumerable) -> x ]
    member x.Objects = objects
    member x.AddObject v = objects <- v :: objects
    member x.Transform (f: 'T -> 'T) = objects <- List.map f objects
    member x.IterateObjects (f: 'T -> unit) = List.iter (unbox >> f) objects

