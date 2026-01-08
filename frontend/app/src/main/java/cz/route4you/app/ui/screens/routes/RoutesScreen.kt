@file:OptIn(androidx.compose.material3.ExperimentalMaterial3Api::class)

package cz.route4you.app.ui.screens.routes

import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.lazy.items
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material.icons.filled.MoreVert
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp
import cz.route4you.app.domain.ClimbRoute

@Composable
fun RoutesScreen(
    vm: RoutesViewModel,
    areaId: String,
    onBack: () -> Unit,
    onRouteClick: (String) -> Unit,
    onCreateRouteClick: (String) -> Unit,
    onEditRouteClick: (String) -> Unit,
    onDeleteRouteClick: (String) -> Unit
) {
    val state by vm.state.collectAsState()

    LaunchedEffect(areaId) { vm.load(areaId) }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Cesty") },
                navigationIcon = {
                    IconButton(onClick = onBack) {
                        Icon(Icons.AutoMirrored.Filled.ArrowBack, contentDescription = "Zpět")
                    }
                }
            )
        },
        floatingActionButton = {
            FloatingActionButton(onClick = { onCreateRouteClick(areaId) }) { Text("+") }
        }
    ) { padding ->
        Box(
            modifier = Modifier
                .padding(padding)
                .fillMaxSize()
        ) {
            when {
                state.isLoading -> CircularProgressIndicator(Modifier.padding(16.dp))
                state.error != null -> Text("Chyba: ${state.error}", Modifier.padding(16.dp))
                else -> LazyColumn(
                    contentPadding = PaddingValues(12.dp),
                    verticalArrangement = Arrangement.spacedBy(8.dp)
                ) {
                    items(state.routes) { route ->
                        RouteItem(
                            route = route,
                            onOpen = { onRouteClick(route.id) },
                            onEdit = { onEditRouteClick(route.id) },
                            onDelete = { onDeleteRouteClick(route.id) }
                        )
                    }
                }
            }
        }
    }
}

@Composable
private fun RouteItem(
    route: ClimbRoute,
    onOpen: () -> Unit,
    onEdit: () -> Unit,
    onDelete: () -> Unit
) {
    var menuOpen by remember(route.id) { mutableStateOf(false) }
    var confirmOpen by remember(route.id) { mutableStateOf(false) }

    Card(
        modifier = Modifier.fillMaxWidth()
    ) {
        Row(
            Modifier
                .fillMaxWidth()
                .padding(12.dp),
            horizontalArrangement = Arrangement.SpaceBetween
        ) {
            Column(
                modifier = Modifier
                    .weight(1f)
                    .clickable(onClick = onOpen)
            ) {
                Text(route.name, style = MaterialTheme.typography.titleMedium)

                val ratingText = route.ratingAvg?.let { String.format("%.1f", it) } ?: "-"

                Text(
                    "⭐ $ratingText (${route.ratingsCount}) • Ascents: ${route.ascentsCount}",
                    style = MaterialTheme.typography.bodySmall
                )
            }

            Text(route.grade, style = MaterialTheme.typography.titleMedium)

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
                        text = { Text("Delete") },   // ✅ už není disabled
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
            title = { Text("Smazat cestu?") },
            text = { Text("Cesta bude odstraněna (později můžeš přidat i mazání ascentů).") },
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