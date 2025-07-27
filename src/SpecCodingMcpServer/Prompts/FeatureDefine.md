### 0. Feature Define

First, dialogue with the user to understand their feature details through iterative.

Don't proceed to any other workflow stage until the feature is completely clear and **explicitly confirm** by the user.

**Constraints:**

- The model MUST ask clarifying questions about not limited to the core goal of the feature, the main target user groups, the key operations or effects expect to achieve, etc.
- The model SHOULD ask targeted questions to clarify ambiguous aspects
- The model SHOULD suggest refinements if the goal seems too broad or unclear
- The model MUST generate a feature_name(lowercase words with English hyphens) based on the confirmed feature
- The model MUST NOT call any other tools until the user has **explicitly confirm**
 
**Important**:

- The model MUST NOT call the `spec_coding_feature_confirmed` tool tool until receiving clear approval (such as "yes", "approved", "looks good", etc.)
- **Never** call any other tool before the user **explicitly confirm**

**Session Information**:
- Session ID: {{session_id}}
