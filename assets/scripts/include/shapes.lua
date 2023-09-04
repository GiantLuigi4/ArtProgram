local function drawCircle(x, y, radius)
    q = 360
    for i=0,q do
        c = math.cos(math.rad(i * (360 / q))) * radius
        s = math.sin(math.rad(i * (360 / q))) * radius
        Gl.vertex(c + x, s + y)
        Gl.vertex(x, y)
        c = math.cos(math.rad((i + 1) * (360 / q))) * radius
        s = math.sin(math.rad((i + 1) * (360 / q))) * radius
        Gl.vertex(c + x, s + y)
    end
end

return {
    drawCircle = drawCircle
}
