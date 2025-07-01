using UnityEngine;

[RequireComponent (typeof(AudioSource))]
public class MapAudioEqualizerEffector : MonoBehaviour
{
    [SerializeField] private GameObject _unitPrefab;
    [SerializeField] private float _unitLengthZ = 1f;
    [SerializeField] private Vector3 _spawningOffset;
    [SerializeField, Range(64, 512)] private int _spawnCount = 128;
    [SerializeField] private float _scaleYMax = 5f;
    [SerializeField] private float _gamma = 2f;
    [SerializeField] private bool _doShuffle = true;

    private AudioSource _source;
    private Transform[] _spawnedUnits;
    private float[] _spectrumDatum;

    private void Awake()
    {
        _source = GetComponent<AudioSource>();
        _spawnedUnits = new Transform[_spawnCount];
        _spectrumDatum = new float[_spawnCount];

        for(int i = 0; i < _spawnCount; i++)
        {
            Transform spawned = Instantiate(_unitPrefab).transform;
            spawned.position = _spawningOffset + new Vector3(0, 0, i * _unitLengthZ);
            _spawnedUnits[i] = spawned;
        }
    }

    private void Update()
    {
        _source.GetSpectrumData(_spectrumDatum, 0, FFTWindow.BlackmanHarris);

        if(_doShuffle)
        {
            _spectrumDatum.Shuffle();
        }

        _spectrumDatum[0] = _spectrumDatum[_spawnCount -1] = 0f;
        _spawnedUnits[0].localScale = _spawnedUnits[_spawnCount -1].localScale = new Vector3(1f, 0.01f, 1f);

        for(int i = 1; i < _spawnCount - 1; i++)
        {
            _spectrumDatum[i] = Mathf.Log(1f + _spectrumDatum[i], 2) * _scaleYMax; // TODO : Max Scale 로 보정해야함
            _spectrumDatum[i] = Mathf.Pow(_spectrumDatum[i], _gamma); // 스펙트럼 증폭
            _spectrumDatum[i] = (_spectrumDatum[i - 1] + _spectrumDatum[i] + _spectrumDatum[i + 1]) / 3f;
            float prevScaleY = _spawnedUnits[i].localScale.y;
            _spawnedUnits[i].localScale = new Vector3(1, Mathf.Lerp(prevScaleY, _spectrumDatum[i], Time.deltaTime * 30), 1);
        }
    }
}
