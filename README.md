# unity-n-database

This is a high level API for working with databases in unity using
the async API based on promises.

## Usage

See the tests in the `Editor/` folder for each class for more examples.

The `test` folder contains a stand-alone test project.

## Install

From your unity project folder:

    npm init
    npm install shadowmint/unity-n-database --save
    echo Assets/packages >> .gitignore
    echo Assets/packages.meta >> .gitignore

The package and all its dependencies will be installed in
your Assets/packages folder.

## Development

Setup and run tests:

    npm install
    npm install ..
    cd test
    npm install

Remember that changes made to the test folder are not saved to the package
unless they are copied back into the source folder.

To reinstall the files from the src folder, run `npm install ..` again.

### Tests

All tests are wrapped in `#if ...` blocks to prevent test spam.

You can enable tests in: Player settings > Other Settings > Scripting Define Symbols

The test key for this package is: `N_DATABASE_TESTS`
