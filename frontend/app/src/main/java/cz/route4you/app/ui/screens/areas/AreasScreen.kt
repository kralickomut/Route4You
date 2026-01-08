@file:OptIn(androidx.compose.material3.ExperimentalMaterial3Api::class)

package cz.route4you.app.ui.screens.areas

import androidx.compose.foundation.clickable
import androidx.compose.foundation.horizontalScroll
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.foundation.rememberScrollState
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material.icons.filled.MoreVert
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.text.style.TextDecoration
import androidx.compose.ui.unit.dp
import cz.route4you.app.domain.Area


@Composable
fun AreasScreen(
    vm: AreasViewModel,
    parentId: String?,
    onBack: () -> Unit,
    onBreadcrumbClick: (String?) -> Unit,
    onAreaClick: (Area) -> Unit,
    onLeafArea: (String) -> Unit,
    onCreateClick: (String?) -> Unit,
    onEditAreaClick: (String) -> Unit,
    onDeleteAreaClick: (String) -> Unit
) {
    val state by vm.state.collectAsState()

    LaunchedEffect(parentId) { vm.load(parentId) }

    // Redirect to routes only for leaf types (Crag/Sector) and only if no subareas
    LaunchedEffect(
        state.isLoading,
        state.error,
        state.areas,
        state.currentAreaId,
        state.currentAreaType
    ) {
        val t = state.currentAreaType?.lowercase()
        val isLeaf = t != null && t != "country" && t != "region" // tolerantní pro seed

        if (isLeaf &&
            state.currentAreaId != null &&
            !state.isLoading &&
            state.error == null &&
            state.areas.isEmpty()
        ) {
            onLeafArea(state.currentAreaId!!)
        }
    }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text(state.title) },
                navigationIcon = {
                    if (parentId != null) {
                        IconButton(onClick = onBack) {
                            Icon(Icons.AutoMirrored.Filled.ArrowBack, contentDescription = "Back")
                        }
                    }
                }
            )
        },
        floatingActionButton = {
            FloatingActionButton(onClick = { onCreateClick(parentId) }) { Text("+") }
        }
    ) { padding ->
        Column(
            Modifier
                .padding(padding)
                .fillMaxSize()
        ) {
            BreadcrumbRow(state.breadcrumbs, onBreadcrumbClick)

            Box(Modifier.fillMaxSize()) {
                when {
                    state.isLoading -> CircularProgressIndicator(Modifier.padding(16.dp))
                    state.error != null -> Text("Chyba: ${state.error}", Modifier.padding(16.dp))
                    else -> AreaList(
                        areas = state.areas,
                        onAreaClick = onAreaClick,
                        onEditAreaClick = onEditAreaClick,
                        onDeleteAreaClick = onDeleteAreaClick
                    )
                }
            }
        }
    }
}

@Composable
private fun BreadcrumbRow(
    crumbs: List<BreadcrumbItem>,
    onClick: (String?) -> Unit
) {
    if (crumbs.isEmpty()) return

    Row(
        modifier = Modifier
            .padding(horizontal = 12.dp, vertical = 8.dp)
            .horizontalScroll(rememberScrollState())
            .fillMaxWidth()
    ) {
        crumbs.forEachIndexed { idx, c ->
            TextButton(
                onClick = { onClick(c.areaId) },
                contentPadding = PaddingValues(horizontal = 4.dp, vertical = 0.dp)
            ) {
                Text(text = c.title, textDecoration = TextDecoration.Underline)
            }
            if (idx != crumbs.lastIndex) Text(">", modifier = Modifier.padding(horizontal = 2.dp))
        }
    }
}

@Composable
private fun AreaList(
    areas: List<Area>,
    onAreaClick: (Area) -> Unit,
    onEditAreaClick: (String) -> Unit,
    onDeleteAreaClick: (String) -> Unit
) {
    LazyColumn(
        contentPadding = PaddingValues(12.dp),
        verticalArrangement = Arrangement.spacedBy(8.dp)
    ) {
        items(areas) { area ->
            AreaItem(
                area = area,
                onOpen = { onAreaClick(area) },
                onEdit = { onEditAreaClick(area.id) },
                onDelete = { onDeleteAreaClick(area.id) }
            )
        }
    }
}

@Composable
private fun AreaItem(
    area: Area,
    onOpen: () -> Unit,
    onEdit: () -> Unit,
    onDelete: () -> Unit
) {
    var menuOpen by remember(area.id) { mutableStateOf(false) }
    var confirmOpen by remember(area.id) { mutableStateOf(false) }

    Card(
        modifier = Modifier.fillMaxWidth()
    ) {
        Row(
            modifier = Modifier
                .fillMaxWidth()
                .padding(12.dp),
            horizontalArrangement = Arrangement.SpaceBetween
        ) {
            // ✅ Klikatelné je jen tohle vlevo – ne celý Card
            Column(
                modifier = Modifier
                    .weight(1f)
                    .clickable(onClick = onOpen)
            ) {
                Text(area.name, style = MaterialTheme.typography.titleMedium)
                Text(
                    "${area.type} • ${area.childrenCount} subareas • ${area.routesCount} routes",
                    style = MaterialTheme.typography.bodySmall
                )
            }

            Box {
                IconButton(onClick = { menuOpen = true }) {
                    Icon(Icons.Filled.MoreVert, contentDescription = "Menu")
                }

                DropdownMenu(
                    expanded = menuOpen,
                    onDismissRequest = { menuOpen = false }
                ) {
                    DropdownMenuItem(
                        text = { Text("Edit") },
                        onClick = {
                            menuOpen = false
                            onEdit()
                        }
                    )
                    DropdownMenuItem(
                        text = { Text("Delete") },
                        onClick = {
                            menuOpen = false
                            confirmOpen = true
                        }
                    )
                }
            }
        }
    }

    if (confirmOpen) {
        AlertDialog(
            onDismissRequest = { confirmOpen = false },
            title = { Text("Smazat oblast?") },
            text = { Text("Smaže se tato oblast i všechny její pod-oblasti a cesty.") },
            confirmButton = {
                TextButton(onClick = {
                    confirmOpen = false
                    onDelete()
                }) { Text("Smazat") }
            },
            dismissButton = {
                TextButton(onClick = { confirmOpen = false }) { Text("Zrušit") }
            }
        )
    }
}