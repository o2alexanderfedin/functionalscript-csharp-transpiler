#!/bin/bash

# Git Flow Enforcement Setup Script
# This script sets up git hooks and branch protection for enforcing git flow

set -e

# Color codes
RED='\033[0;31m'
YELLOW='\033[1;33m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'
BOLD='\033[1m'
NC='\033[0m'

echo -e "${BOLD}Git Flow Enforcement Setup${NC}"
echo "============================"
echo ""

# 1. Initialize git flow if not already done
echo -e "${BLUE}1. Checking Git Flow initialization...${NC}"
if ! git config --get gitflow.branch.master &>/dev/null; then
    echo "   Git Flow not initialized. Initializing now..."
    
    # Check if main or master exists
    if git rev-parse --verify main &>/dev/null; then
        PROD_BRANCH="main"
    elif git rev-parse --verify master &>/dev/null; then
        PROD_BRANCH="master"
    else
        PROD_BRANCH="main"
    fi
    
    git flow init -d
    git config gitflow.branch.master "$PROD_BRANCH"
    echo -e "${GREEN}   ✓ Git Flow initialized with production branch: $PROD_BRANCH${NC}"
else
    echo -e "${GREEN}   ✓ Git Flow already initialized${NC}"
fi

# 2. Set up git hooks
echo ""
echo -e "${BLUE}2. Installing Git Hooks...${NC}"

# Create hooks directory if it doesn't exist
mkdir -p .git/hooks

# Install hooks from .githooks directory
if [ -d ".githooks" ]; then
    for hook in .githooks/*; do
        if [ -f "$hook" ]; then
            hook_name=$(basename "$hook")
            cp "$hook" ".git/hooks/$hook_name"
            chmod +x ".git/hooks/$hook_name"
            echo -e "   ${GREEN}✓ Installed $hook_name hook${NC}"
        fi
    done
else
    echo -e "   ${YELLOW}⚠️  .githooks directory not found${NC}"
fi

# Alternative: Configure git to use .githooks directory
git config core.hooksPath .githooks
echo -e "   ${GREEN}✓ Configured git to use .githooks directory${NC}"

# 3. Set up local branch protection (using git config)
echo ""
echo -e "${BLUE}3. Setting up local branch protection...${NC}"

# Protect main/master and develop branches
PROD_BRANCH=$(git config --get gitflow.branch.master || echo "main")
DEV_BRANCH=$(git config --get gitflow.branch.develop || echo "develop")

git config branch.$PROD_BRANCH.protected true
git config branch.$DEV_BRANCH.protected true
echo -e "   ${GREEN}✓ Protected branches: $PROD_BRANCH, $DEV_BRANCH${NC}"

# 4. GitHub branch protection (if gh CLI is available)
echo ""
echo -e "${BLUE}4. Setting up GitHub branch protection...${NC}"

if command -v gh &>/dev/null; then
    # Get repository info
    REPO=$(gh repo view --json nameWithOwner -q .nameWithOwner 2>/dev/null || echo "")
    
    if [ -n "$REPO" ]; then
        echo "   Setting protection for $REPO..."
        
        # Protect main/master branch
        gh api repos/$REPO/branches/$PROD_BRANCH/protection \
            --method PUT \
            --field required_status_checks='{"strict":true,"contexts":["continuous-integration"]}' \
            --field enforce_admins=false \
            --field required_pull_request_reviews='{"dismiss_stale_reviews":true,"require_code_owner_reviews":false,"required_approving_review_count":1}' \
            --field restrictions=null \
            --silent 2>/dev/null && \
            echo -e "   ${GREEN}✓ Protected $PROD_BRANCH on GitHub${NC}" || \
            echo -e "   ${YELLOW}⚠️  Could not protect $PROD_BRANCH (may need admin rights)${NC}"
        
        # Protect develop branch
        gh api repos/$REPO/branches/$DEV_BRANCH/protection \
            --method PUT \
            --field required_status_checks='{"strict":false,"contexts":[]}' \
            --field enforce_admins=false \
            --field required_pull_request_reviews=null \
            --field restrictions=null \
            --silent 2>/dev/null && \
            echo -e "   ${GREEN}✓ Protected $DEV_BRANCH on GitHub${NC}" || \
            echo -e "   ${YELLOW}⚠️  Could not protect $DEV_BRANCH (may need admin rights)${NC}"
    else
        echo -e "   ${YELLOW}⚠️  Not in a GitHub repository${NC}"
    fi
else
    echo -e "   ${YELLOW}⚠️  GitHub CLI not installed. Install with: brew install gh${NC}"
fi

# 5. Create git aliases for common git flow commands
echo ""
echo -e "${BLUE}5. Creating helpful git aliases...${NC}"

git config alias.feature "flow feature"
git config alias.release "flow release"
git config alias.hotfix "flow hotfix"
git config alias.fs "flow feature start"
git config alias.ff "flow feature finish"
git config alias.rs "flow release start"
git config alias.rf "flow release finish"
git config alias.hs "flow hotfix start"
git config alias.hf "flow hotfix finish"

echo -e "   ${GREEN}✓ Created git aliases:${NC}"
echo "     git fs <name>  = git flow feature start"
echo "     git ff <name>  = git flow feature finish"
echo "     git rs <ver>   = git flow release start"
echo "     git rf <ver>   = git flow release finish"
echo "     git hs <ver>   = git flow hotfix start"
echo "     git hf <ver>   = git flow hotfix finish"

# 6. Display summary
echo ""
echo -e "${BOLD}${GREEN}✅ Git Flow Enforcement Setup Complete!${NC}"
echo ""
echo -e "${YELLOW}What's been configured:${NC}"
echo "• Git hooks prevent direct commits to protected branches"
echo "• Commit messages must follow Conventional Commits format"
echo "• Push operations validate git flow compliance"
echo "• Branch protection rules (local and GitHub if available)"
echo "• Helpful git aliases for git flow commands"
echo ""
echo -e "${BLUE}Quick Reference:${NC}"
echo "Start feature:  git flow feature start <name>"
echo "Start release:  git flow release start <version>"
echo "Start hotfix:   git flow hotfix start <version>"
echo ""
echo -e "${YELLOW}💡 Tips:${NC}"
echo "• Hooks are in .githooks/ directory"
echo "• Run this script again to update hooks"
echo "• Use 'git config --unset core.hooksPath' to disable hooks temporarily"
echo ""