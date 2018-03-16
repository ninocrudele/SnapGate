Connect-ServiceFabricCluster -ConnectionEndpoint localhost:19000

$nodes = Get-ServiceFabricNode

foreach($node in $nodes)
{
    $replicas = Get-ServiceFabricDeployedReplica -NodeName $node.NodeName -ApplicationName "fabric:/GrabCaster"

    foreach ($replica in $replicas)
    {
        Remove-ServiceFabricReplica -ForceRemove -NodeName $node.NodeName -PartitionId $replica.Partitionid -ReplicaOrInstanceId $replica.ReplicaOrInstanceId
    }
}