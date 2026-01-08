@file:OptIn(androidx.compose.material3.ExperimentalMaterial3Api::class)

package cz.route4you.app.ui.screens.editarea

import androidx.compose.foundation.layout.*
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.automirrored.filled.ArrowBack
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Modifier
import androidx.compose.ui.unit.dp

@Composable
fun EditAreaScreen(
    vm: EditAreaViewModel,
    areaId: String,
    onBack: () -> Unit,
    onSaved: () -> Unit
) {
    val state by vm.state.collectAsState()

    LaunchedEffect(areaId) { vm.load(areaId) }

    Scaffold(
        topBar = {
            TopAppBar(
                title = { Text("Upravit oblast") },
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