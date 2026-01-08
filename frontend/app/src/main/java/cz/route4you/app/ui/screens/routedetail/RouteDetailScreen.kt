@file:OptIn(androidx.compose.material3.ExperimentalMaterial3Api::class)

package cz.route4you.app.ui.screens.routedetail

import androidx.compose.foundation.horizontalScroll
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.rememberScrollState
import androidx.compose.ui.text.style.TextDecoration
import androidx.compose.ui.unit.dp

@Composable
fun RouteDetailScreen(
    vm: RouteDetailViewModel,
    routeId: String,
    onBack: () -> Unit,
    onBreadcrumbClick: (String?) -> Unit,
    onRecordAscentClick: (String) -> Unit
) {
    val state by vm.state.collectAsState()


    val snackbarHostState = remember { SnackbarHostState() }

    // načtení detailu
    LaunchedEffect(routeId) { vm.load(routeId) }


    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Detail cesty") },
                navigationIcon = {
                    IconButton(onClick = onBack) {
                        Icon(Icons.AutoMirrored.Filled.ArrowBack, contentDescription = "Back")
                    }
                }
            )
        },
        snackbarHost = { SnackbarHost(snackbarHostState) } // ✅ tady
    ) { padding ->

        Box(
            modifier = Modifier
                .padding(padding)
                .fillMaxSize()
        ) {
            when {
                state.isLoading -> CircularProgressIndicator(Modifier.padding(16.dp))

                state.error != null -> Text("Chyba: ${state.error}", Modifier.padding(16.dp))

                state.route != null -> {
                    val r = state.route!!
                    val ratingText =
                        if (r.ratingAvg != null) String.format("%.1f", r.ratingAvg) else "-"

                    Column(
                        Modifier.padding(16.dp),
                        verticalArrangement = Arrangement.spacedBy(10.dp)
                    ) {
                        BreadcrumbRow(state.breadcrumbs, onBreadcrumbClick)

                        Text("${r.name}  ${r.grade}", style = MaterialTheme.typography.headlineSmall)

                        Text("Style: ${r.style} • Pitches: ${r.pitches} • Length: ${r.lengthMeters ?: "-"} m")
                        Text("Ascents: ${r.ascentsCount}")
                        Text("Rating: $ratingText (${r.ratingsCount})")

                        if (r.tags.isNotEmpty()) {
                            Text("Tags: ${r.tags.joinToString(", ")}", style = MaterialTheme.typography.bodySmall)
                        }

                        Spacer(Modifier.height(8.dp))

                        Button(
                            onClick = { onRecordAscentClick(routeId) },
                            modifier = Modifier.fillMaxWidth()
                        ) {
                            Text("I climbed it")
                        }

                    }
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
    Row(
        modifier = Modifier
            .horizontalScroll(rememberScrollState())
            .fillMaxWidth()
    ) {
        crumbs.forEachIndexed { index, crumb ->
            TextButton(
                onClick = { onClick(crumb.areaId) },
                contentPadding = PaddingValues(horizontal = 4.dp, vertical = 0.dp)
            ) {
                Text(
                    text = crumb.title,
                    textDecoration = TextDecoration.Underline
                )
            }

            if (index != crumbs.lastIndex) {
                Text(" > ", modifier = Modifier.padding(horizontal = 2.dp))
            }
        }
    }
}

