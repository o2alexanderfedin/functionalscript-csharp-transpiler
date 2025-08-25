# Git Flow Enforcement Documentation

## Overview
This repository enforces Git Flow methodology through automated hooks and branch protection to maintain code quality and consistent workflow.

## 🚀 Quick Setup

Run the setup script to enable all enforcement mechanisms:
```bash
./setup-gitflow-enforcement.sh
```

## 🛡️ Enforcement Mechanisms

### 1. Git Hooks
Located in `.githooks/` directory:

#### **pre-commit**
- ❌ Blocks direct commits to `main`/`master` and `develop` branches
- ⚠️ Warns about non-git-flow branch names
- 📝 Validates commit message format (optional)

#### **pre-push**
- ❌ Prevents direct pushes to protected branches
- ✅ Validates git flow branch naming
- 💡 Provides helpful git flow command hints

#### **commit-msg**
- 📝 Enforces Conventional Commits format
- ✅ Valid types: feat, fix, docs, style, refactor, test, chore, perf, ci, build, revert
- ⚠️ Warns about commit message length
- 🚨 Detects BREAKING CHANGE annotations

### 2. Branch Protection

#### Local Protection
- Git config marks branches as protected
- Hooks check branch protection status

#### GitHub Protection (via gh CLI)
- **main/master**: Requires PR reviews, status checks
- **develop**: Less restrictive, allows feature merges
- Enforced through GitHub API

## 📚 Git Flow Commands Reference

### Feature Development
```bash
git flow feature start <feature-name>    # Start new feature
git flow feature finish <feature-name>   # Merge to develop
git flow feature publish <feature-name>  # Push to remote
git flow feature pull origin <feature>   # Pull from remote
```

### Release Management
```bash
git flow release start <version>         # Start release from develop
# ... bump version, update changelog ...
git flow release finish <version>        # Merge to main & develop, create tag
git flow release publish <version>       # Share release branch
```

### Hotfix Management
```bash
git flow hotfix start <version>          # Start hotfix from main
# ... apply critical fix ...
git flow hotfix finish <version>         # Merge to main & develop, create tag
```

## 🎯 Workflow Examples

### Starting a New Feature
```bash
# ❌ WRONG - Direct branch creation
git checkout -b my-feature

# ✅ CORRECT - Using git flow
git flow feature start my-feature
```

### Making a Release
```bash
# ❌ WRONG - Manual merge to main
git checkout main
git merge develop

# ✅ CORRECT - Using git flow
git flow release start 1.2.0
# Update version files, changelog
git flow release finish 1.2.0
```

### Emergency Hotfix
```bash
# ❌ WRONG - Fix directly on main
git checkout main
# make fixes
git commit -m "fix: critical bug"

# ✅ CORRECT - Using git flow
git flow hotfix start 1.2.1
# make fixes
git commit -m "fix: critical bug"
git flow hotfix finish 1.2.1
```

## 💡 Git Aliases

The setup script creates helpful aliases:
```bash
git fs <name>  # git flow feature start
git ff <name>  # git flow feature finish
git rs <ver>   # git flow release start
git rf <ver>   # git flow release finish
git hs <ver>   # git flow hotfix start
git hf <ver>   # git flow hotfix finish
```

## 🔧 Troubleshooting

### Bypass Hooks (Emergency Only!)
```bash
# Disable hooks temporarily
git config --unset core.hooksPath

# Single commit bypass (NOT RECOMMENDED)
git commit --no-verify -m "emergency: bypass hooks"
```

### Re-enable Hooks
```bash
git config core.hooksPath .githooks
# Or run setup script again
./setup-gitflow-enforcement.sh
```

### Check Git Flow Status
```bash
git flow config        # Show git flow configuration
git branch -a          # Show all branches
git config --list | grep gitflow  # Show git flow settings
```

## 📋 Conventional Commits Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Types
- **feat**: New feature
- **fix**: Bug fix
- **docs**: Documentation only
- **style**: Formatting, missing semicolons, etc
- **refactor**: Code restructuring without changing behavior
- **test**: Adding missing tests
- **chore**: Maintenance tasks
- **perf**: Performance improvements
- **ci**: CI/CD changes
- **build**: Build system changes
- **revert**: Reverting previous commit

### Examples
```bash
feat(auth): add OAuth2 integration
fix(api): handle null response correctly
docs: update README with new examples
chore: bump version to 2.0.0
```

## 🚨 Important Notes

1. **Never force push** to protected branches
2. **Always use git flow commands** for branch operations
3. **Follow Conventional Commits** for clear history
4. **Update version files** during releases
5. **Test locally** before pushing

## 📊 Branch Strategy

```
main (production)
  ├── hotfix/1.0.1
  └── release/1.1.0
      └── develop (integration)
          ├── feature/user-auth
          ├── feature/api-redesign
          └── feature/new-dashboard
```

## 🔍 Verification

To verify enforcement is working:
```bash
# Try to commit directly to main (should fail)
git checkout main
echo "test" > test.txt
git add test.txt
git commit -m "test"  # This should be blocked!

# Try invalid commit message (should fail)
git checkout -b feature/test
echo "test" > test.txt
git add test.txt
git commit -m "bad message"  # This should fail validation!
```

## 📚 Resources

- [Git Flow Cheatsheet](https://danielkummer.github.io/git-flow-cheatsheet/)
- [Conventional Commits](https://www.conventionalcommits.org/)
- [Semantic Versioning](https://semver.org/)
- [GitHub Branch Protection](https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/defining-the-mergeability-of-pull-requests/about-protected-branches)