@file:OptIn(androidx.compose.material3.ExperimentalMaterial3Api::class)

package cz.route4you.app.ui.screens.createroute

import androidx.compose.foundation.layout.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp

@Composable
fun CreateRouteScreen(
    vm: CreateRouteViewModel,
    areaId: String,
    onBack: () -> Unit,
    onSaved: (createdRouteId: String) -> Unit
) {
    val state by vm.state.collectAsState()

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Nová cesta") },
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
            OutlinedTextField(
                value = state.name,
                onValueChange = vm::setName,
                label = { Text("Název cesty") },
                modifier = Modifier.fillMaxWidth()
            )

            OutlinedTextField(
                value = state.grade,
                onValueChange = vm::setGrade,
                label = { Text("Obtížnost (např. 6c)") },
                modifier = Modifier.fillMaxWidth()
            )

            OutlinedTextField(
                value = state.pitchesText,
                onValueChange = vm::setPitches,
                label = { Text("Délky (pitches)") },
                modifier = Modifier.fillMaxWidth()
            )

            OutlinedTextField(
                value = state.lengthText,
                onValueChange = vm::setLength,
                label = { Text("Délka v metrech (volitelné)") },
                modifier = Modifier.fillMaxWidth()
            )

            OutlinedTextField(
                value = state.style,
                onValueChange = vm::setStyle,
                label = { Text("Style (Sport/Trad/Boulder)") },
                modifier = Modifier.fillMaxWidth()
            )

            OutlinedTextField(
                value = state.tagsText,
                onValueChange = vm::setTags,
                label = { Text("Tagy (odděl čárkou)") },
                modifier = Modifier.fillMaxWidth()
            )

            state.error?.let { Text("Chyba: $it") }

            Button(
                onClick = { vm.save(areaId, onSaved) },
                enabled = !state.isSaving,
                modifier = Modifier.fillMaxWidth()
            ) {
                Text(if (state.isSaving) "Ukládám..." else "Uložit")
            }
        }
    }
}