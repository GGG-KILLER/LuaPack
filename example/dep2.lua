print 'dep2 running'

local z = dofile './dep3.lua'
return {
    a = z.a + 1,
    b = z.a + 1
}