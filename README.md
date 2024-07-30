# CounterStrikeSharp
This repository included my own modifications for it to suits my use case better

## How to Build
Building requires CMake.

Clone the repository
```bash
git clone https://github.com/roflmuffin/counterstrikesharp
```

Init and update submodules
```bash
git submodule update --init --recursive
```

Make build folder
```bash
mkdir build
cd build
```

Generate CMake Build Files
```bash
cmake ..
```

Build
```bash
cmake --build . --config Debug
```

License
-------
CounterStrikeSharp is licensed under the GNU General Public License version 3. A special exemption is outlined regarding published plugins, which you can find in the [LICENSE](LICENSE) file.
