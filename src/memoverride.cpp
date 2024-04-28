#include "memalloc.h"

void* __cdecl operator new(const size_t nSize) { return MemAlloc_Alloc(nSize); }
void* __cdecl operator new[](const size_t nSize) { return MemAlloc_Alloc(nSize); }
void __cdecl operator delete(void* pMemory) noexcept { g_pMemAlloc->Free(pMemory); }
void __cdecl operator delete[](void* pMemory) noexcept { g_pMemAlloc->Free(pMemory); }
