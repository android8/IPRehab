[**Compiler Options in MSBuild**](https://www.staging-typescript.org/docs/handbook/compiler-options-in-msbuild.html)

When you have an MSBuild based project which utilizes TypeScript such as an ASP.NET Core project, you can configure TypeScript in two ways. Either via a tsconfig.json or via the project settings.

[**What is TypeScript**](https://www.staging-typescript.org/docs/handbook/tsconfig-json.htm)

The presence of a tsconfig.json file in a directory indicates that the directory is the root of a TypeScript project. The tsconfig.json file specifies the root files and the compiler options required to compile the project.

JavaScript projects can use a jsconfig.json file instead, which acts almost the same but has some JavaScript-related compiler flags enabled by default.

[**Consumption**](https://www.typescriptlang.org/docs/handbook/declaration-files/consumption.html)


__underline__
Getting type declarations requires no tools apart from npm.

As an example, getting the declarations for a library like jquery takes nothing more than the following command

npm install --save-dev @types/jquery
It is worth noting that if the npm package already includes its declaration file as described in Publishing, downloading the corresponding @types package is not needed.

__Consuming__
From there you’ll be able to use lodash in your TypeScript code with no fuss. This works for both modules and global code.

For example, once you’ve npm install-ed your type declarations, you can use imports and write

import * as _ from "lodash";
_.padStart("Hello TypeScript!", 20, " ");
or if you’re not using modules, you can just use the global variable _.

_.padStart("Hello TypeScript!", 20, " ");