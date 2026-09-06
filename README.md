# HISP (C# / Angular rewrite)

HISP is a small client-server image processing app. A user uploads an
image in the browser, picks a processing operation, and the backend runs
that operation using a native C/C++ image-processing library before
sending the result back to be displayed.

This is a work in progress — see [Current status](#current-status) and
[Known limitations](#known-limitations) below.

## How it works

```
┌─────────────────────┐        multipart/form-data        ┌───────────────────────┐        P/Invoke        ┌──────────────────────────┐
│  Angular frontend    │  ───────────────────────────────▶ │  ASP.NET Core backend  │ ──────────────────────▶ │  Native HISP image lib   │
│  (hisp-frontend)     │ ◀─────────────────────────────── │  (HispBackend)         │ ◀────────────────────── │  (libHISPImageProcessing)│
└─────────────────────┘        processed image (PNG)       └───────────────────────┘        pixel buffer       └──────────────────────────┘
```

- **Frontend** (`frontend/hisp-frontend`): an Angular app where a user can
  enter their name, upload an image, choose an operation, and (for blur
  operations) set a kernel radius and/or sigma. Submitting sends the image
  and parameters to the backend and displays the processed result that
  comes back.
- **Backend** (`backend/HispBackend`): an ASP.NET Core Web API
  (`ImageController`) that accepts the uploaded image, decodes it with
  [StbImageSharp](https://github.com/StbSharp/StbImageSharp), calls into a
  native shared library to do the actual pixel processing, and re-encodes
  the result as a PNG with StbImageWriteSharp.
- **Native library** (`backend/HISP` git submodule, built to
  `backend/build/libHISPImageProcessing.so`): C/C++ code exposing
  `image_blur`, `image_gaussian_blur`, and `image_grayscale`, called from
  the backend via `[LibraryImport]`/P-Invoke.
- **Tests** (`backend/HispBackend.Tests`): xUnit tests covering the image
  processing service and controller.

### Supported operations

| Operation | Frontend value | Backend endpoint | Extra params |
|---|---|---|---|
| Simple box blur | `blur` | `POST /api/Image/process/blur` | `kernelRadius` (1–13) |
| Gaussian blur | `gblur` | `POST /api/Image/process/gblur` | `kernelRadius` (odd, 1–13), `sigma` (≥ 0) |
| Grayscale | `grayscale` | `POST /api/Image/process/grayscale` | — |

There's also a generic `POST /api/Image/process` endpoint that currently
just logs metadata about the uploaded image (width/height/type) without
returning a processed result — this looks like scaffolding for future
work.

## Current status

This is an early-stage / learning project, not a finished product:

- Only three operations exist so far (box blur, Gaussian blur, grayscale).
- The user's "name" field on the frontend is decorative — it isn't sent to
  the backend anywhere.
- Several code paths (a pure-C# reference implementation of the blur/
  grayscale algorithms) are kept around, commented out, alongside the
  native-library calls that replaced them.

## Known limitations

- **The native image library lives in a separate, private submodule**
  (`backend/HISP`, mapped via SSH in `.gitmodules`). If you don't have
  access to that repository you won't be able to build
  `libHISPImageProcessing.so` yourself; you'll need the maintainer to grant
  access or to hand you a prebuilt `.so`. Everything below assumes you have
  it.
- **The `.so` is Linux-only** (`LibraryImport("libHISPImageProcessing.so", ...)`),
  so the backend currently only runs on Linux (or WSL). There's no macOS
  (`.dylib`) or Windows (`.dll`) build yet.
- **CORS isn't configured** on the backend. The Angular dev server (port
  `4200`) calling the API (port `5192`) may be blocked by the browser
  unless you add CORS middleware in `Program.cs` or otherwise proxy the
  requests.
- **The backend URL is hardcoded** in the frontend
  (`submit-button.ts` posts to `http://localhost:5192/...`), so both
  services are expected to run on `localhost` with those exact ports.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (backend targets `net10.0`)
- [Node.js](https://nodejs.org/) and npm (frontend uses npm `11.17.0`, Angular `22.x`)
- A C/C++ toolchain (e.g. `gcc`/`clang` + `make` or CMake — whatever the
  `backend/HISP` submodule uses) to build the native library, unless
  you're given a prebuilt `libHISPImageProcessing.so`
- Git, with access to the private `backend/HISP` submodule repository
- Linux or WSL (see [Known limitations](#known-limitations))

## Setup

### 1. Clone the repository (with submodules)

```bash
git clone --recurse-submodules https://github.com/twagner9/hisp_csharp.git
cd hisp_csharp
```

If you already cloned without `--recurse-submodules`, pull the submodule
in separately (you'll need access to `backend/HISP`):

```bash
git submodule update --init --recursive
```

### 2. Build the native image-processing library

Build the `backend/HISP` submodule per its own instructions, and make
sure the resulting shared library ends up at:

```
backend/build/libHISPImageProcessing.so
```

Both `HispBackend.csproj` and `HispBackend.Tests.csproj` reference that
exact relative path and copy it into their output directories.

### 3. Run the backend

```bash
cd backend
dotnet restore
dotnet run --project HispBackend
```

By default this listens on `http://localhost:5192` (see
`Properties/launchSettings.json`). You can hit
`backend/HispBackend/HispBackend.http` from an IDE with a REST client for
quick manual testing.

To run the test suite instead:

```bash
dotnet test
```

### 4. Run the frontend

In a separate terminal:

```bash
cd frontend/hisp-frontend
npm install
npm start
```

This runs `ng serve`, which serves the Angular app at
`http://localhost:4200` by default.

### 5. Use the app

1. Open `http://localhost:4200` in your browser.
2. Upload an image.
3. Pick an operation (Simple Blur, Gaussian Blur, or Grayscale) and set
   any required parameters (kernel radius / sigma).
4. Click **Submit**. The processed image should appear once the backend
   responds.

If the request fails in the browser console with a CORS error, add CORS
middleware to `backend/HispBackend/Program.cs` allowing
`http://localhost:4200` (see [Known limitations](#known-limitations)).
