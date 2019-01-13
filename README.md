<p align="center">
 <img src="https://github.com/Boomshicleafaunda/Support_BaseplateRules/blob/master/title.png" height=64>
</p>

<p align="center"><sup><i>Adds baseplate alignment and adjacency rules for building</i></sup></p>

<p align="center"><strong>Release:</strong> 1.0.0</p>

## About Baseplate Rules

Baseplate Rules provide Alignment and Adjacency rules for building. When Baseplate Rules are enabled, all players will be required to start all of their builds on a baseplate. When Baseplate Alignment is enabled, the starting baseplate must be aligned to a configurable grid. When Baseplate Adjacency is enabled, the starting baseplate must be placed adjacent (directly next to) another baseplate.

## Preferences for Baseplate Rules

Baseplate Rules are completely configurable. Here are all of the preferences:

### Baseplate Preferences

#### Baseplate Required

When enabled, players are required to start all of their builds on a baseplate.

> **Note:** You can use the `/baseplaterules enable` command require baseplates. Similarly, you can use the `/baseplaterules disable` command to no longer require baseplates. By default, baseplate rules are enabled.

#### Baseplate Type

It can be configured such that not all baseplates from the "Baseplates" category can be used as starting baseplates. Using this preference, you can configure the starting baseplate to be one of three types:

 - **Any:** Any brick from the baseplate category.
 - **Plate:** Any 1/3x tall plate brick from the baseplate category.
 - **Plain:** Any brick from the Plain baseplate subcategory.
 
 > **Note:** You can use the `/baseplaterules type set <type>` command to set the required baseplate type. By default, the required baseplate type is set to "Any".

### Adjacency Preferences

#### Use Adjacency

When enabled, players are required to place their starting baseplate adjacent to another baseplate.

> **Note:** You can use the `/baseplaterules adjacency enable` command to enable adjacency rules. Similarly, you can use the `/baseplaterules adjacency disable` command to disable adjacency rules. By default, adjacency rules are enabled.

#### Include Diagonals

When enabled, baseplates diagonally positioned are considered adjacent.

> **Note:** You can use the `/baseplaterules adjacency diagonals enable` command to allow diagonal adjacency. Similarly, you can use the `/baseplaterules adjacency diagonals disable` command to no longer allow diagonal adjacency. By default, diagonal adjacency is not allowed.

### Alignment Preferences

#### Use Alignment

When enabled, players are required to align their baseplates to a configurable grid.

> **Note:** You can use the `/baseplaterules alignment enable` command to enable alignment rules. Similarly, you can use the `/baseplaterules alignment disable` command to disable alignment rules. By default, alignment rules are enabled.

#### Auto Alignment

When enabled, players attempting to place a misaligned baseplate will have their ghost brick automatically realigned for them.

> **Note:** You can use the `/baseplaterules alignment auto enable` command to enable auto alignment. Similarly, you can use the `/baseplaterules alignment auto disable` command to disable auto alignment. By default, auto alignment is enabled.

#### Grid Cell Size

The alignment grid is configurable in such a way that allows you to control the grid cell size.

> **Note:** You can use the `/baseplaterules alignment grid set <X> <Y>` command to update the grid cell size. Both the `<X>` and `<Y>` values must be valid integers between 1 and 1024. By default, the grid cell size is set to 16x16.

#### Grid Offset

The alignment grid is configurable in such a way that allows you to control the grid offset. This should really only be used when trying to snap the grid to a preexisting build or terrain-based obstable. In an flat freebuild world, adjusting the grid offset is unnecessary.

> **Note:** You can use the `/baseplaterules alignment offset set <X> <Y>` command to update the grid offset. Both the `<X>` and `<Y>` values must be valid integers between 0 and 1024. By default, the grid offset is set to 0x0.
