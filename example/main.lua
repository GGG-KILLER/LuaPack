local z = dofile "dep3.lua"
local y = dofile "dep2.lua"
local x = dofile "dep1.lua"
print(x.a + x.b,y.a + y.b,z.a + z.b)