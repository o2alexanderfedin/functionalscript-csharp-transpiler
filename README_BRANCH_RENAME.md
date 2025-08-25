# Branch Rename Instructions

## Renaming master to main

The local repository has been updated to use `main` instead of `master`. To complete the migration:

### 1. On GitHub:
1. Go to Settings → Branches
2. Change the default branch from `master` to `main`
3. Update any branch protection rules

### 2. After changing default on GitHub:
```bash
# Delete the old master branch from remote
git push origin --delete master
```

### 3. For other contributors:
They should update their local repositories:
```bash
# Fetch the latest branches
git fetch origin

# Rename local master to main
git branch -m master main

# Set main to track origin/main
git branch -u origin/main main

# Update gitflow configuration
git config --local gitflow.branch.master main
```

### Current Configuration:
- Local default branch: `main`
- Gitflow configured to use `main` as production branch
- Both `main` and `master` exist on remote (need to delete `master` after changing default on GitHub)