# Engine.ImageMagick

Engine.ImageMagick is a .NET 9 API that implements a simple microservice wrapper for the [ImageMagick](https://imagemagick.org/) library.

ImageMagick is a heavy assembly with very specific and focused usage in our platform. Recently, a security vulnerability forced an update to this library, which was an unexpected (and unnecessary) interruption to our daily workflow. Wrapping this dependency behind an API allows us to remove it from our core build and deployment pipeline. We can upgrade the component (when necessary) as a separate and distinct activity.

## Proposed Improvements

In the future, when time and opportunity permit, the following improvements should be considered:

### Rename Project

Rename the project to `Shift.Kernel.Integration.ImageMagick` so the project name follows current coding conventions.