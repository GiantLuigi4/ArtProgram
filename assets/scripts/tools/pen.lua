ox = 0
oy = 0
dragging = false
lastPressure = 0.1

shapes = require("include.shapes")

function setup(worker, color)
    sdr = Gl.getShader("util/colored")
    sdr.bind()
    Gl.setupUniforms(sdr)
    sdr.setUniformF("Color", 0, 0, 0, 1)

    worker.startWrite()
    Gl.begin()

    return sdr
end

function finish(sdr, worker)
    Gl.finish()
    worker.stopWrite()

    sdr.unbind()
end

function calcPressure(x, ox, y, oy, usePressure)
    local pressure = math.sqrt((x - ox) * (x - ox) + (y - oy) * (y - oy)) / 20
    pressure = math.max(pressure, 0.1)
    pressure = (pressure + lastPressure + lastPressure + lastPressure) / 4
    if not usePressure then
        pressure = 1
    end
    return pressure
end

function onDrag(x, y, tex, worker, color, width, usePressure)
    if (dragging) then
        local pressure = calcPressure(x, ox, y, oy, usePressure)
        local radius = width

        -- line smoothing
        local nx = (ox + x) / 2
        local ny = (oy + y) / 2

        -- draw
        local sdr = setup(worker, color)
        q = 360/4
        for i=0,q do
            c = math.cos(math.rad(i * (360 / q)))
            s = math.sin(math.rad(i * (360 / q)))
            -- current circle
            Gl.vertex(c * pressure * radius + nx, s * pressure * radius + ny)
            -- without this, gaps get left behind
            Gl.vertex(nx, ny)
            Gl.vertex(ox, oy)
            -- previous circle
            Gl.vertex(c * lastPressure * radius + ox, s * lastPressure * radius + oy)
        end
        finish(sdr, worker)
        
        sdr = setup(worker, color)
        shapes.drawCircle(nx, ny, radius * pressure)
        finish(sdr, worker)

        lastPressure = pressure
        ox = nx
        oy = ny
    end
end

function onClick(x, y, tex, worker, color, width, usePressure)
    ox = x
    oy = y
    lastPressure = 0.1
    dragging = true

    local radius = width
    if usePressure then
        radius = radius * 0.1
    end

    -- draw
    local sdr = setup(worker, color)
    shapes.drawCircle(x, y, radius)
    finish(sdr, worker)
end

function onRelease(x, y, tex, worker, color, width, usePressure)
    dragging = false

    local sdr = setup(worker, color)
    local radius = width
    local pressure = calcPressure(x, ox, y, oy, usePressure)

    -- draw
    q = 360/4
    for i=0,q do
        c = math.cos(math.rad(i * (360 / q)))
        s = math.sin(math.rad(i * (360 / q)))
        -- current circle
        Gl.vertex(c * pressure * radius + x, s * pressure * radius + y)
        -- without this, gaps get left behind
        Gl.vertex(x, y)
        Gl.vertex(ox, oy)
        -- previous circle
        Gl.vertex(c * lastPressure * radius + ox, s * lastPressure * radius + oy)
    end
    finish(sdr, worker)
    
    sdr = setup(worker, color)
    shapes.drawCircle(x, y, radius * pressure)
    finish(sdr, worker)

    ox = 0
    oy = 0
end

return {
    iconSheet = "buttons.png",
    iconX = 0,
    iconY = 0,
    iconRes = 48,

    args = {
        {
            name = "color",
            type = "color",
            default = 0
        },
        {
            name = "width",
            type = "double",
            min = 0,
            default = 10
        },
        {
            name = "usePressure",
            type = "boolean",
            default = true
        }
    }
}
