@file:OptIn(androidx.compose.material3.ExperimentalMaterial3Api::class)

package cz.route4you.app.ui.screens.editroute

import androidx.compose.foundation.layout.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp

@Composable
fun EditRouteScreen(
    vm: EditRouteViewModel,
    routeId: String,
    onBack: () -> Unit,
    onSaved: () -> Unit
) {
    val state by vm.state.collectAsState()

    LaunchedEffect(routeId) { vm.load(routeId) }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Upravit cestu") },
                navigationIcon = {
                    IconButton(onClick = onBack) {
                        Icon(Icons.AutoMirrored.Filled.ArrowBack, contentDescription = "Back")
                    }
                }
            )
        }
    ) { padding ->
        Column(
            Modifier.padding(padding).padding(16.dp),
            verticalArrangement = Arrangement.spacedBy(12.dp)
        ) {
            if (state.isLoading) {
                CircularProgressIndicator()
                return@Column
            }

            OutlinedTextField(
                value = state.name,
                onValueChange = vm::setName,
                label = { Text("Název") },
                modifier = Modifier.fillMaxWidth()
            )

            OutlinedTextField(
                value = state.grade,
                onValueChange = vm::setGrade,
                label = { Text("Obtížnost") },
                modifier = Modifier.fillMaxWidth()
            )

            OutlinedTextField(
                value = state.pitchesText,
                onValueChange = vm::setPitches,
                label = { Text("Pitches") },
                modifier = Modifier.fillMaxWidth()
            )

            OutlinedTextField(
                value = state.lengthText,
                onValueChange = vm::setLength,
                label = { Text("Length (m)") },
                modifier = Modifier.fillMaxWidth()
            )

            OutlinedTextField(
                value = state.style,
                onValueChange = vm::setStyle,
                label = { Text("Style") },
                modifier = Modifier.fillMaxWidth()
            )

            OutlinedTextField(
                value = state.tagsText,
                onValueChange = vm::setTags,
                label = { Text("Tags (comma separated)") },
                modifier = Modifier.fillMaxWidth()
            )

            state.error?.let { Text("Chyba: $it") }

            Button(
                onClick = { vm.save(onSaved) },
                enabled = !state.isSaving,
                modifier = Modifier.fillMaxWidth()
            ) {
                Text(if (state.isSaving) "Ukládám..." else "Uložit")
            }
        }
    }
}