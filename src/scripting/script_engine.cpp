/*
 * Copyright (c) 2014 Bas Timmer/NTAuthority et al.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 *
 * This file has been modified from its original form for use in this program
 * under GNU Lesser General Public License, version 2.
 */

#include "scripting/script_engine.h"

#include <stack>
#include <unordered_map>
#include <vector>
#include <mutex>

#include "core/log.h"
#include "core/utils.h"

static std::unordered_map<uint64_t, counterstrikesharp::TNativeHandler> g_registeredHandlers;
static thread_local std::stack<std::string> errors;

namespace counterstrikesharp {

void ScriptContext::ThrowNativeError(const char* msg, ...)
{
    va_list arglist;
    va_start(arglist, msg);

    int size = vsnprintf(nullptr, 0, msg, arglist);
    va_end(arglist);

    if (size < 0)
    {
        errors.push("Formatting error in ThrowNativeError");
        this->SetResult(errors.top().c_str());
        *this->m_has_error = 1;
        return;
    }

    std::vector<char> error_string(size + 1);

    va_start(arglist, msg);
    vsnprintf(error_string.data(), size + 1, msg, arglist);
    va_end(arglist);

    errors.push(std::string(error_string.data()));

    const char* ptr = errors.top().c_str();
    this->SetResult(ptr);
    *this->m_has_error = 1;
}

void ScriptContext::Reset()
{
    if (*m_has_error && !errors.empty())
    {
        errors.pop();
    }

    m_numResults = 0;
    m_numArguments = 0;
    *m_has_error = 0;

    std::fill(std::begin(m_native_context->arguments), std::end(m_native_context->arguments), 0);
    m_native_context->result = 0;
}

tl::optional<TNativeHandler> ScriptEngine::GetNativeHandler(uint64_t nativeIdentifier)
{
    static thread_local std::unordered_map<uint64_t, TNativeHandler>::iterator lastIt = g_registeredHandlers.end();
    if (lastIt != g_registeredHandlers.end() && lastIt->first == nativeIdentifier)
    {
        return lastIt->second;
    }

    auto it = g_registeredHandlers.find(nativeIdentifier);
    if (it != g_registeredHandlers.end())
    {
        lastIt = it;
        return it->second;
    }

    return tl::optional<TNativeHandler>();
}

tl::optional<TNativeHandler> ScriptEngine::GetNativeHandler(std::string identifier)
{
    return GetNativeHandler(hash_string(identifier.c_str()));
}

bool ScriptEngine::CallNativeHandler(uint64_t nativeIdentifier, ScriptContext& context)
{
    auto h = GetNativeHandler(nativeIdentifier);
    if (h)
    {
        (*h)(context);

        return true;
    }

    return false;
}

void ScriptEngine::RegisterNativeHandlerInt(uint64_t nativeIdentifier, TNativeHandler function)
{
    g_registeredHandlers[nativeIdentifier] = function;
}

void ScriptEngine::InvokeNative(counterstrikesharp::fxNativeContext& context)
{
    if (context.nativeIdentifier == 0) return;

    auto nativeHandler = counterstrikesharp::ScriptEngine::GetNativeHandler(context.nativeIdentifier);

    if (nativeHandler)
    {
        counterstrikesharp::ScriptContextRaw scriptContext(context);

        (*nativeHandler)(scriptContext);
    }
    else
    {
        CSSHARP_CORE_WARN("Native Handler was requested but not found: {0:x}", context.nativeIdentifier);
        assert(false);
    }
}

ScriptContextRaw ScriptEngine::m_context;

} // namespace counterstrikesharp
