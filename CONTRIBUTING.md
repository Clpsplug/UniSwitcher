# Contribution Guide

Contributors are asked to adhere to the following for easier processing of their contributions.


## Reporting an issue

* First, search the issues for what you are trying to report.  
* If you don't find any duplicate issues, use the issue template to report your issue; 
    when clicking "new issue", several options should appear (e.g., Bug report.)  
* Try to fill as much information in the template.
* Keep the scope of an issue to one 'issue'. Avoid cramming multiple independent issues into one.

## Submitting a PR

* PR should have a linked issue. If there's none, consider making one first.
    * Don't forget an [issue-closing keyword](https://docs.github.com/en/issues/tracking-your-work-with-issues/linking-a-pull-request-to-an-issue#linking-a-pull-request-to-an-issue-using-a-keyword) if you want to close the issue in question
* PR title must be a bite-size description of what your PR does. Do not leave it as the branch name.
* Tiny fixes like typo fixes should be squashed.
* As soon as you notice that your `base` branch gets out of date when you have a PR, please `git pull --rebase origin base`.
* PR branches should be deleted when the PR is merged/closed.

## Code style

* Install the EditorConfig plugin for your IDE/text editor.
* Member variable name conventions:
    |Visibility|Case|Sample|
    |:---------|:---|:---|
    |private (pure C#)|`_lowerCamelCase`|`private int _someInteger`|
    |private (Unity field)|`lowerCamelCase`|`[SerializeField] private int someInteger`|
    |protected|`UpperCamelCase`|`protected int SomeInteger`|
    |public|`UpperCamelCase`|`public int SomeInteger`|
* Unless absolutely needed, declare Unity fields as `private`/`protected`
