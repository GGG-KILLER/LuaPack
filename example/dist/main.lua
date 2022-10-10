local __packer_import_file_funcs, __packer_import = {}
do
    local __packer_import_cache, __packer_import_cache_nil_value = {}, {}
    function __packer_import(name, ...)
        local cached = __packer_import_cache[name]
        if cached == nil then
            local file = assert(__packer_import_file_funcs[name],"file not found")
            cached = file(...)
            if cached == nil then
                cached = __packer_import_cache_nil_value
            end
            __packer_import_cache[name] = cached
        end
        if cached == __packer_import_cache_nil_value then
            cached = nil
        end
        return cached
    end
end
__packer_import_file_funcs["example/dep1.lua"] = function(...)
    print 'dep1 running'
    local y = __packer_import "example/dep2.lua"
    local z = __packer_import "example/dep3.lua"
    return { a = y.a + z.a + 1, b = y.b + z.a + 1 }
end
__packer_import_file_funcs["example/dep2.lua"] = function(...)
    print 'dep2 running'
    local z = __packer_import "example/dep3.lua"
    return { a = z.a + 1, b = z.a + 1 }
end
__packer_import_file_funcs["example/dep3.lua"] = function(...)
    print 'dep3 running'
    return { a = 1, b = 1 }
end
__packer_import_file_funcs["example/main.lua"] = function(...)
    local z = __packer_import "example/dep3.lua"
    local y = __packer_import "example/dep2.lua"
    local x = __packer_import "example/dep1.lua"
    print(x.a + x.b,y.a + y.b,z.a + z.b)
end
__packer_import "example/main.lua"