print 'dep1 running'

local y = dofile './dep2.lua'
local z = dofile './dep3.lua'
return {
    a = y.a + z.a + 1,
    b = y.b + z.a + 1
}